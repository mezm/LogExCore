using Microsoft.Extensions.Logging;
using System;

namespace ExtLog.NET.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("ExtLog.NET.Sample", LogLevel.Debug)
                    .AddSingleLineConsole();
                    //.AddConsole();
            });

            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Press ENTER");
            Console.ReadLine();
        }
    }
}
