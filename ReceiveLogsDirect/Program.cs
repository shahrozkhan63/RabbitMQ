﻿using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);
// declare a server-named queue
var queueName = channel.QueueDeclare().QueueName;

// Automatically bind to 'info', 'warning', and 'error'
var severities = new[] { "info", "warning", "error" };

foreach (var severity in severities)
{
    channel.QueueBind(queue: queueName,
                      exchange: "direct_logs",
                      routingKey: severity);
}

Console.WriteLine(" [*] Waiting for messages.");

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

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
