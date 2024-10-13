using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Send
{
    public static void Main()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "task_queue",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

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


            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;


            // Send the message to the RabbitMQ queue
            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "task_queue",
                                 basicProperties: properties,
                                 body: body);

            Console.WriteLine($" [x] Sent '{message}'");
        }
    }
}
