using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Configuration
{
    public static class Rabbit
    {
        public static void Send<TEvent>(TEvent @event)
        {
            var factory = new ConnectionFactory()
            {
                HostName = ConfigReader.Get(ConfigReader.RabbitHost),
                Password = ConfigReader.Get(ConfigReader.RabbitPassword),
                UserName = ConfigReader.Get(ConfigReader.RabbitUser)
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: typeof(TEvent).FullName, type: "fanout");

                    string message = JsonConvert.SerializeObject(@event);
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: typeof(TEvent).FullName,
                                         routingKey: "",
                                         basicProperties: null,
                                         body: body);
                }
            }
        }

        public static void RegisterEventHandler<TEvent>(Action<TEvent> handler)
        {
            var factory = new ConnectionFactory
            {
                HostName = ConfigReader.Get(ConfigReader.RabbitHost),
                Password = ConfigReader.Get(ConfigReader.RabbitPassword),
                UserName = ConfigReader.Get(ConfigReader.RabbitUser)
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: typeof(TEvent).FullName, type: "fanout");

                    var queueName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queue: queueName,
                                      exchange: typeof(TEvent).FullName,
                                      routingKey: "");

                    Console.WriteLine(" [*] Waiting for logs.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var @event = JsonConvert.DeserializeObject<TEvent>(message);
                        handler(@event);
                        Console.WriteLine(" [x] {0}", message);
                    };
                    channel.BasicConsume(queue: queueName,
                                         noAck: true,
                                         consumer: consumer);

                }
            }
        }
    }
}