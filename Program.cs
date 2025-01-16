using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Newtonsoft.Json;

var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };

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
    // Console.WriteLine($" [x] Received: {message}");
    // Simulate some async processing if needed

    try
    {
        // Deserialize the message to Product object
        var product = JsonConvert.DeserializeObject<Product>(message);
        if (product != null)
        {
            Console.WriteLine($" [x] Received Product:");
            Console.WriteLine($"     ID: {product.ProductId}");
            Console.WriteLine($"     Name: {product.ProductName}");
            Console.WriteLine($"     Description: {product.ProductDescription}");
            Console.WriteLine($"     Price: {product.ProductPrice}");
            Console.WriteLine($"     Stock: {product.ProductStock}");
        }
        else
        {
            Console.WriteLine(" [!] Received message could not be deserialized.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($" [!] Error processing message: {ex.Message}");
    }
    //await Task.Yield();
};

// Start consuming messages asynchronously
await channel.BasicConsumeAsync(queue: "product", autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public int ProductPrice { get; set; }
    public int ProductStock { get; set; }
}