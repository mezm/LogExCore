using FactoryBot;
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

        public static void Main()
        {
            Bot.Define(x => new LogMessage { Text = x.Strings.Any(), Delay = x.Integer.Any(100, 5000) });
            Bot.Define(x => new LogMessageWithException 
            { 
                Text = x.Strings.Words(), 
                Delay = x.Integer.Any(1000, 10000), 
                ExceptionMessage = x.Strings.Any(),
                Level = x.Enums.RandomFromList(LogLevel.Warning, LogLevel.Error, LogLevel.Critical)
            }).AfterPropertyBinding(x => x.Text = "EX: " + x.Text);

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
                .ToList();

            tasks.Add(RunLogExceptionProcessAsync(cancelation.Token));

            System.Console.ReadLine();
            cancelation.Cancel();

            Task.WaitAll(tasks.ToArray());
        }

        private static async Task RunLogProcessAsync(LogLevel level, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var msg = Bot.Build<LogMessage>();
                Logger.Log(level, msg.Text);
                await Task.Delay(msg.Delay);
            }
        }

        private static async Task RunLogExceptionProcessAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var msg = Bot.Build<LogMessageWithException>();
                try
                {
                    throw new Exception(msg.ExceptionMessage);
                } 
                catch (Exception ex)
                {
                    Logger.Log(msg.Level, ex, msg.Text);
                }
                await Task.Delay(msg.Delay);
            }
        }

        private class LogMessage
        {
            public string Text { get; set; }

            public int Delay { get; set; }
        }

        private class LogMessageWithException : LogMessage
        {
            public LogLevel Level { get; set; }

            public string ExceptionMessage { get; set; }
        }
    }
}
