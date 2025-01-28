using System.Text;
using RabbitMQ.Client;

var factory  = new ConnectionFactory() { HostName = "localhost" };
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);

while (true)
{
    Console.Write("Input the value you want to publish: ");
    string? message = Console.ReadLine();

    if (!string.IsNullOrEmpty(message))
    {
        var body = Encoding.UTF8.GetBytes(message!);
        await channel.BasicPublishAsync(exchange: "logs", routingKey: string.Empty, body: body);
        Console.WriteLine($" [x] Sent {message}");
    }
}
