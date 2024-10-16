using System;
using System.Text;
using System.Threading.Channels;
using Newtonsoft.Json;
using PublisherConfirm;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Declare the exchange
var validExchange = "product_exchange";
channel.ExchangeDeclare(exchange: validExchange, type: ExchangeType.Direct, durable: true, autoDelete: false);

// Enable publisher confirms
channel.ConfirmSelect();

// Set up the return event for messages that can't be routed
channel.BasicReturn += (sender, args) =>
{
    var returnMessage = Encoding.UTF8.GetString(args.Body.ToArray());
    Console.WriteLine($" [!] Message '{returnMessage}' was returned from the broker.");
};

// Create a Product instance
var product = new Product
{
    Id = 1,
    Name = "Sample Product 3",
    Price = 99.99M,
    Description = "This is a sample product.",
    CategoryName = "Electronics",
    ImageURL = "http://example.com/sample.jpg"
};

// Delay before publishing
Console.WriteLine("Waiting for 10 seconds before publishing...");
Thread.Sleep(10000); // Wait for 10 seconds

// Serialize the Product object to JSON
var message = JsonConvert.SerializeObject(product);
var body = Encoding.UTF8.GetBytes(message);

// Attempt to publish a message
channel.BasicPublish(exchange: validExchange, // Use a valid exchange
                     routingKey: "product",      // Routing key
                     basicProperties: null,
                     body: body,
                     mandatory: true);            // Set mandatory to true

// Wait for confirmation with a short timeout
if (channel.WaitForConfirms(TimeSpan.FromMilliseconds(100)))
{
    Console.WriteLine("[x] Message successfully confirmed.");
}
else
{
    Console.WriteLine("[!] Message failed to confirm.");
}


/*
// Use an existing exchange but deliberately incorrect routing key to force a return
var exchange = "confirm_logs";  // Assume this exists
var invalidRoutingKey = "non_existent_key";  // Invalid routing key


while (true)
{
    Console.WriteLine("Enter the severity (e.g., 'info', 'warning', 'error'), or type 'exit' to quit:");
    var severity = Console.ReadLine();

    if (severity == "exit")
        break;

    Console.WriteLine("Enter the message:");
    var message = Console.ReadLine();

    var body = Encoding.UTF8.GetBytes(message);

    Console.WriteLine("Waiting for 5 seconds before publishing...");
    Thread.Sleep(3000); // Wait for 5 seconds

    // Publish with mandatory flag
    channel.BasicPublish(exchange: exchange,
                      routingKey: severity,
                      basicProperties: null,
                      body: body,
                      mandatory: true);  // Set mandatory flag to true

    Console.WriteLine($" [x] Message '{severity}':'{message}' sent with delivery tag {channel.NextPublishSeqNo}");

    // Wait for confirmation
    if (channel.WaitForConfirms(TimeSpan.FromMilliseconds(1)))
    {
        Console.WriteLine($" [x] Message with delivery tag {channel.NextPublishSeqNo - 1} successfully confirmed.");
    }
    else
    {
        Console.WriteLine($" [!] Message with delivery tag {channel.NextPublishSeqNo - 1} failed to confirm.");
    }
}
*/
