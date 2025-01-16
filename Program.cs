using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };

// Create the RabbitMQ connection asynchronously
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// Declare the queue asynchronously
await channel.QueueDeclareAsync(queue: "product", durable: false, exclusive: false, autoDelete: false, arguments: null);

// Create and configure the consumer
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received: {message}");
    // Simulate some async processing if needed
    await Task.Yield();
};

// Start consuming messages asynchronously
await channel.BasicConsumeAsync(queue: "hello", autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

