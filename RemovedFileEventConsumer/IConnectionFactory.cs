using RabbitMQ.Client;

namespace RemovedFileEventConsumer
{
    public interface IConnectionFactory
    {
        IConnection Connection { get; }
    }
}