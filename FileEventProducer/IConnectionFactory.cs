using RabbitMQ.Client;

namespace FileEventProducer
{
    public interface IConnectionFactory
    {
        IConnection Connection { get; }
    }
}