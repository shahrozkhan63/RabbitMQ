using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);

do
{
    // Get routing key from user input
    Console.WriteLine("Enter the routing key (e.g., 'anonymous.info'), or type 'exit' to quit:");
    var routingKey = Console.ReadLine();

    // Check if user wants to exit
    if (routingKey?.ToLower() == "exit") break;

    if (string.IsNullOrWhiteSpace(routingKey))
    {
        Console.WriteLine("Routing key cannot be empty. Try again.");
        continue;
    }

    // Get message from user input
    Console.WriteLine("Enter the message, or type 'exit' to quit:");
    var message = Console.ReadLine();

    // Check if user wants to exit
    if (message?.ToLower() == "exit") break;

    if (string.IsNullOrWhiteSpace(message))
    {
        Console.WriteLine("Message cannot be empty. Try again.");
        continue;
    }

    var body = Encoding.UTF8.GetBytes(message);

    // Publish the message with the given routing key
    channel.BasicPublish(exchange: "topic_logs",
                         routingKey: routingKey,
                         basicProperties: null,
                         body: body);

    Console.WriteLine($" [x] Sent '{routingKey}':'{message}'");

} while (true);

Console.WriteLine("Exiting. Press [enter] to close.");
Console.ReadLine();
