using RabbitMQ.Client;

namespace ReceivedFileEventConsumer
{
    public interface IConnectionFactory
    {
        IConnection Connection { get; }
    }
}