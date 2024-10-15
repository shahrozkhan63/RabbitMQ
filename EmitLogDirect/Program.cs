using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

string severity, message;

do
{
    // Get severity from user input
    Console.WriteLine("Enter severity (info, warning, error) or 'exit' to quit:");
    severity = Console.ReadLine();

    // Exit the loop if the user types 'exit'
    if (severity?.ToLower() == "exit") break;

    // Get message from user input
    Console.WriteLine("Enter the log message:");
    message = Console.ReadLine() ?? "Hello World!";

    var body = Encoding.UTF8.GetBytes(message);

    // Publish the message with the routing key (severity)
    channel.BasicPublish(exchange: "direct_logs",
                         routingKey: severity,
                         basicProperties: null,
                         body: body);

    Console.WriteLine($" [x] Sent '{severity}':'{message}'");

} while (true);

Console.WriteLine("Exiting. Press [enter] to close.");
Console.ReadLine();
