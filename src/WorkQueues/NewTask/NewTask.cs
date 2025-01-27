using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost", };
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "work_queues", durable: true, exclusive: false, autoDelete: false,
    arguments: null);

string message = GetMessage(args);
var body = Encoding.UTF8.GetBytes(message);
var properties = new BasicProperties { Persistent = true, };

await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "work_queues", mandatory: true,
    basicProperties: properties, body: body);
return;

static string GetMessage(string[] args) =>
    ((args.Length > 0) ? string.Join(" ", args) : "Work Queues!");
