using System;
using Configuration;
using EasyNetQ;

namespace Infrastructure.Messaging
{
    public static class Rabbit
    {
        public static void Publish<TEvent>(TEvent @event) where TEvent : class
        {
            var bus = RabbitHutch.CreateBus(ConfigReader.RabbitConnectionString);
            bus.Publish(@event, typeof(TEvent).FullName);
        }

        public static void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class
        {
            var bus = RabbitHutch.CreateBus(ConfigReader.RabbitConnectionString);
            bus.Subscribe(ConfigReader.RabbitSubscriptionId, handler);
        }
    }
}