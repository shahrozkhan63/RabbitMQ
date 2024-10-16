﻿using System;
using System.Text;
using RabbitMQ.Client;

class Send
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "hello",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        Console.WriteLine("Enter the message you want to send:");
        string message = Console.ReadLine();
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: string.Empty,
                             routingKey: "hello",
                             basicProperties: null,
                             body: body);
        Console.WriteLine($" [x] Sent '{message}'");

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
