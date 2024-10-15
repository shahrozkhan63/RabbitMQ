using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);

// declare a server-named queue
var queueName = channel.QueueDeclare().QueueName;

do
{
    // Get binding keys from user input
    Console.WriteLine("Enter binding keys (comma separated, e.g., 'kern.*', '*.critical'), or type 'exit' to quit:");
    var input = Console.ReadLine();

    // Check if user wants to exit
    if (input?.ToLower() == "exit") break;

    var bindingKeys = input.Split(',').Select(key => key.Trim()).ToArray();

    if (bindingKeys.Length < 1)
    {
        Console.Error.WriteLine("You must specify at least one binding key.");
        continue;
    }

    foreach (var bindingKey in bindingKeys)
    {
        channel.QueueBind(queue: queueName,
                          exchange: "topic_logs",
                          routingKey: bindingKey);
    }

    Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var routingKey = ea.RoutingKey;
        Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
    };

    channel.BasicConsume(queue: queueName,
                         autoAck: true,
                         consumer: consumer);

} while (true);

Console.WriteLine("Exiting. Press [enter] to close.");
Console.ReadLine();
