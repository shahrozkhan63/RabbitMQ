using System;
using System.Text;
using Newtonsoft.Json;
using PublisherConfirm;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var validExchange = "product_exchange";

channel.ExchangeDeclare(validExchange, ExchangeType.Direct, durable: true, autoDelete: false);
var queueName = channel.QueueDeclare().QueueName;

// Bind the queue to the exchange for multiple severity levels
channel.QueueBind(queue: queueName, exchange: validExchange, routingKey: "info.product.*");
channel.QueueBind(queue: queueName, exchange: validExchange, routingKey: "product");
//channel.QueueBind(queue: queueName, exchange: validExchange, routingKey: "error");

Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    // Deserialize the JSON message back into a Product object
    var product = JsonConvert.DeserializeObject<Product>(message);
    Console.WriteLine($" [x] Received Product: Id={product.Id}, Name={product.Name}, Price={product.Price}, " +
                      $"Description={product.Description}, Category={product.CategoryName}, ImageURL={product.ImageURL}");
    var routingKey = ea.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
};

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
