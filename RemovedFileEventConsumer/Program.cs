using System;
using System.IO;
using Autofac;
using LoggingImplementations;
using LoggingInterfaces;
using Microsoft.Extensions.Configuration;

namespace RemovedFileEventConsumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            var config = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json")
                                .AddEnvironmentVariables()
                                .Build();
#else
            var assemblyName = System.Reflection.Assembly.GetEntryAssembly().Location;
            var assemblyDir = assemblyName.Replace(Path.GetFileName(assemblyName), string.Empty);

            var config = new ConfigurationBuilder()
                                .SetBasePath(assemblyDir)
                                .AddJsonFile("appsettings.json")
                                .AddEnvironmentVariables()
                                .Build();
#endif

            var containerBuilder = new ContainerBuilder();

            containerBuilder
                .Register(context => config)
                .As<IConfigurationRoot>();

            containerBuilder
                .Register(context => NLogLogger.GetLogger())
                .As<ILogger>();

            containerBuilder
                .RegisterType<RabbitConnectionFactory>()
                .As<IConnectionFactory>();

            containerBuilder
                .RegisterType<RemovedFileEventConsumerService>();

            using (var container = containerBuilder.Build())
            using (var consumerService = container.Resolve<RemovedFileEventConsumerService>())
            {
#if DEBUG
                Console.Clear();

                Console.Out.WriteLine("Starting Event Consumer");

                consumerService.Start();

                var shouldExit = false;

                do
                {
                    Console.Out.WriteLine();
                    Console.Out.Write("Press \"x\" and <Enter> to quit: ");
                    var line = Console.In.ReadLine();
                    shouldExit = string.Compare("x", line.Trim(), StringComparison.OrdinalIgnoreCase) == 0;
                } while (!shouldExit);

                Console.Out.WriteLine("Stopping Event Consumer");

                consumerService.Stop();

                Console.Out.WriteLine("Quitting");
#else
                consumerService.Start();

                while (!Environment.HasShutdownStarted)
                    System.Threading.Thread.Sleep(100);

                consumerService.Stop();
#endif
            }
        }
    }
}
