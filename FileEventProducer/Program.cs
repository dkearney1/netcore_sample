using System;
using System.IO;
using Autofac;
using LoggingImplementations;
using LoggingInterfaces;
using Microsoft.Extensions.Configuration;

namespace FileEventProducer
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
                .RegisterType<FileEventProducerService>();

            using (var container = containerBuilder.Build())
            using (var producerService = container.Resolve<FileEventProducerService>())
            {
#if DEBUG
                Console.Clear();

                Console.Out.WriteLine("Starting Event Producer");

                producerService.Start();

                var shouldExit = false;

                do
                {
                    Console.Out.WriteLine();
                    Console.Out.Write("Press \"x\" and <Enter> to quit: ");
                    var line = Console.In.ReadLine();
                    shouldExit = string.Compare("x", line.Trim(), StringComparison.OrdinalIgnoreCase) == 0;
                } while (!shouldExit);

                Console.Out.WriteLine("Stopping Event Producer");

                producerService.Stop();

                Console.Out.WriteLine("Quitting");
#else
                producerService.Start();

                while (!Environment.HasShutdownStarted)
                    System.Threading.Thread.Sleep(100);

                producerService.Stop();
#endif
            }
        }
    }
}
