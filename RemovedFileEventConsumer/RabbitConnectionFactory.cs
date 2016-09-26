using System;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace RemovedFileEventConsumer
{
    public sealed class RabbitConnectionFactory : IConnectionFactory, IDisposable
    {
        private bool _disposedValue; // To detect redundant calls
        private readonly IConnection _connection;

        public RabbitConnectionFactory(IConfigurationRoot config)
        {
            var rabbitConnectionFactory = new ConnectionFactory()
            {
                HostName = config["RabbitConnection:Hostname"],
                Port = int.Parse(config["RabbitConnection:Port"]),
                VirtualHost = config["RabbitConnection:VirtualHost"],
                UserName = config["RabbitConnection:UserName"],
                Password = config["RabbitConnection:Password"],
            };

            rabbitConnectionFactory.AutomaticRecoveryEnabled = true;

            var process = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

            Console.Out.WriteLine($"Connecting to message broker at {rabbitConnectionFactory.HostName}:{rabbitConnectionFactory.Port}");

            _connection = rabbitConnectionFactory.CreateConnection(process);
        }

        public IConnection Connection => _connection;
        #region IDisposable Support

        void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_connection != null)
                        _connection.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RabbitConnectionFactory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}