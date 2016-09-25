using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileEvents;
using LoggingInterfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace ReceivedFileEventConsumer
{
    public sealed class ReceivedFileEventConsumerService : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IModel _channel;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly UTF8Encoding _encoding;

        private bool _disposedValue; // To detect redundant calls
        private Task _task;

        public ReceivedFileEventConsumerService(ILogger logger, IConnectionFactory connectionFactory)
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
                var queueName = CreatePrivateQueueName();
                var qdr = _channel.QueueDeclare(queueName, false, true, false, null);
                _channel.QueueBind(queueName, "FileEvents.FileReceived:FileEvents", "#", null);

                using (var subscription = new Subscription(_channel, queueName, true))
                {
                    while (!cancelToken.IsCancellationRequested)
                    {
                        try
                        {
                            BasicDeliverEventArgs results = null;
                            subscription.Next(100, out results);
                            if (results != null)
                            {
                                 var s = _encoding.GetString(results.Body);
                                var fileReceived = JsonConvert.DeserializeObject<FileReceived>(s);
                                await OnFileReceived(fileReceived);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                        }
                    }
                }

                _channel.QueueUnbind(queueName, "FileEvents.FileReceived:FileEvents", "#", null);

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private Task OnFileReceived(FileReceived fileReceived)
        {
            var now = DateTimeOffset.Now;
            var msg = $"FileReceived: Id={fileReceived.Id}, Filename={Path.GetFileNameWithoutExtension(fileReceived.FullPath)}, Delay={(now - fileReceived.Created).TotalMilliseconds}ms";

            _logger.Info(null, msg);
            Console.Out.WriteLine(msg);

            return Task.CompletedTask;
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

                        if (_channel!=null)
                        {
                            _channel.Close();
                            _channel.Dispose();
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
        // ~ReceivedFileEventConsumerService() {
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