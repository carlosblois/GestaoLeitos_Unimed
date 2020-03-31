using RabbitMQ.Client;
using System;

namespace EventBusRabbitMQUtil
{
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
