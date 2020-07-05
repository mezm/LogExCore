using LogExCore.SingleLineConsole;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;

namespace LogExCore
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddSingleLineConsole(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.AddSingleton<ILoggerProvider, SingleLineConsoleLoggerProvider>();
            LoggerProviderOptions.RegisterProviderOptions<SingleLineConsoleLoggerOptions, SingleLineConsoleLoggerProvider>(builder.Services);
            return builder;
        }

        public static ILoggingBuilder AddSingleLineConsole(this ILoggingBuilder builder, Action<SingleLineConsoleLoggerOptions> configure)
        {
            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddSingleLineConsole();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
