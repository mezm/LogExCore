using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LogExCore.Sample.Console
{
    public class Program
    {
        private static ILogger Logger;

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConfiguration(config.GetSection("Logging"))
                    .AddSingleLineConsole();
            });

            Logger = loggerFactory.CreateLogger<Program>();

            var cancelation = new CancellationTokenSource();
            var tasks = Enum.GetValues(typeof(LogLevel))
                .OfType<LogLevel>()
                .Select(x => RunLogProcessAsync(x, cancelation.Token))
                .ToArray();

            System.Console.ReadLine();
            cancelation.Cancel();

            Task.WaitAll(tasks);
        }

        private static async Task RunLogProcessAsync(LogLevel level, CancellationToken token)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            while (!token.IsCancellationRequested)
            {
                Logger.Log(level, "Message with tick {ticks}", DateTime.Now.Ticks);
                var delay = random.Next(100, 5000);
                await Task.Delay(delay);
            }
        }
    }
}
