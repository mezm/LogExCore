using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExtLog.NET.Sample
{
    public class Program
    {
        private static ILogger _logger;

        public static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("ExtLog.NET.Sample", LogLevel.Debug)
                    .AddSingleLineConsole(x => x.ShowFullLoggerName = true);
            });
            // todo: test config from file

            _logger = loggerFactory.CreateLogger<Program>();

            var cancelation = new CancellationTokenSource();
            var tasks = Enum.GetValues(typeof(LogLevel))
                .OfType<LogLevel>()
                .Select(x => RunLogProcessAsync(x, cancelation.Token))
                .ToArray();
            
            Console.ReadLine();
            cancelation.Cancel();

            Task.WaitAll(tasks);
        }

        private static async Task RunLogProcessAsync(LogLevel level, CancellationToken token)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            while (!token.IsCancellationRequested)
            {
                _logger.Log(level, "Message with tick {ticks}", DateTime.Now.Ticks);
                var delay = random.Next(100, 5000);
                await Task.Delay(delay);
            }
        }
    }
}
