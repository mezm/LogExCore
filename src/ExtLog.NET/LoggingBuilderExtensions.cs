using ExtLog.NET.SingleLineConsoleLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExtLog.NET
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddSingleLineConsole(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, SingleLineConsoleLoggerProvider>();
            return builder;
        }
    }
}
