using System.Text;
using RabbitMQ.Client;


var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

Console.WriteLine("Type 'exit' to stop sending messages.");
while (true)
{
    // Prompt the user for a message
    Console.WriteLine("Enter the message you want to send:");
    string message = Console.ReadLine();

    // Exit the loop if the user types 'exit'
    if (message.ToLower() == "exit")
    {
        Console.WriteLine("Exiting...");
        break;
    }

    // Convert message to byte array
    var body = Encoding.UTF8.GetBytes(message);

    //    var message = GetMessage(args);
    //var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "logs",
                         routingKey: string.Empty,
                         basicProperties: null,
                         body: body);
    Console.WriteLine($" [x] Sent {message}");

}
    Console.ReadLine();

static string GetMessage(string[] args)
{
    return ((args.Length > 0) ? string.Join(" ", args) : "info: Hello World!");
}