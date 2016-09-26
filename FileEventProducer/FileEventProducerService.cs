using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventInterfaces;
using FileEvents;
using LoggingInterfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FileEventProducer
{
    internal sealed class FileEventProducerService : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IModel _channel;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly UTF8Encoding _encoding;

        private bool _disposedValue; // To detect redundant calls
        private Task _task;

        public FileEventProducerService(ILogger logger, IConnectionFactory connectionFactory)
        {
            _logger = logger;
            _channel = connectionFactory.Connection.CreateModel();
            _cancellationTokenSource = new CancellationTokenSource();
            _encoding = new UTF8Encoding(false);
        }

        public void Start()
        {
            _task = Task.Factory.StartNew(DoWork, _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _task.Wait();
            _task = null;
        }

        private string CreatePrivateQueueName()
        {
            var machineName = Environment.MachineName;
            var executable = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            var randomizer = Guid.NewGuid().ToString("N").Substring(0, 8);

            return $"{machineName}_{executable}_{randomizer}";
        }

        private async Task DoWork()
        {
            var cancelToken = _cancellationTokenSource.Token;

            try
            {
                _channel.ExchangeDeclare("FileEvents.FileReceived:FileEvents", "fanout", true, false, null);
                _channel.ExchangeDeclare("FileEvents.FileRemoved:FileEvents", "fanout", true, false, null);

                var eventFileReceived = new FileReceived(@"\\whackity\share\filename.pgp");
                var eventFileRemoved = new FileRemoved(@"\\whackity\share\otherfile.txt");

                while (!cancelToken.IsCancellationRequested)
                {
                    eventFileReceived.Created = DateTimeOffset.Now;
                    eventFileRemoved.Created = DateTimeOffset.Now;

                    Task.WaitAll(
                        new Task[]
                        {
                            PublishEvent(eventFileReceived, "FileEvents.FileReceived:FileEvents"),
                            PublishEvent(eventFileRemoved, "FileEvents.FileRemoved:FileEvents")
                        },
                        cancelToken);

                    // await Task.Delay(TimeSpan.FromSeconds(1d));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private Task PublishEvent<T>(T theEvent, string exchangeName) where T : IEvent
        {
            var json = JsonConvert.SerializeObject(
                theEvent,
                Formatting.None,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All
                });
            var bytes = _encoding.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.DeliveryMode = 1; //non-persistent
            properties.ContentEncoding = "utf-8";
            properties.ContentType = "application/json";
            properties.Type = theEvent.GetType().FullName;

            return Task.Factory.StartNew(() => _channel.BasicPublish(exchangeName, "#", false, properties, bytes));
        }

        #region IDisposable Support
        void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_cancellationTokenSource != null)
                    {
                        _cancellationTokenSource.Cancel();

                        if (_task != null)
                        {
                            _task.Wait();
                            _task = null;
                        }

                        _cancellationTokenSource.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FileEventProducerService() {
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