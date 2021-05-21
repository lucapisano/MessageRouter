using MessageRouter.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Threading;

namespace MessageRouter
{
    class Program
    {
        public const string ConfigurationFileName = "appsettings.json";
        private static ServiceProvider _sp;
        private static RouterEngine _engine;

        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(ConfigurationFileName, false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var sc = new ServiceCollection();
            sc
                .AddLogging(x => x.ClearProviders().SetMinimumLevel(LogLevel.Trace).AddNLog())
                .AddScoped<IConfiguration>(sp => config)
                .AddOptions()
                .AddMessageRouter(config)
                ;

            sc.AddRabbitMqBroker(config);

            _sp = sc.BuildServiceProvider();
            _engine = _sp.GetRequiredService<RouterEngine>();
            _engine.Start();
            var exitEvent = new ManualResetEvent(false);
            Console.WriteLine($"press CTRL+C to stop");
            Console.CancelKeyPress += (sender, eventArgs) => {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };
            exitEvent.WaitOne();
            _engine.Stop();
        }
    }
}
