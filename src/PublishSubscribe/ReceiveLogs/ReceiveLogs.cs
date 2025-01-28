using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Random random = new();
var factory  = new ConnectionFactory() { HostName = "localhost" };
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);
var  queue = await channel.QueueDeclareAsync();
var queueName = queue.QueueName;
await channel.QueueBindAsync(queue: queueName, exchange: "logs", string.Empty);

Console.WriteLine(" [*] Waiting for logs.");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var randomValue= random.Next(1, 5) * 1000;
    await Task.Delay(randomValue);
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] {message}");
};
await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
