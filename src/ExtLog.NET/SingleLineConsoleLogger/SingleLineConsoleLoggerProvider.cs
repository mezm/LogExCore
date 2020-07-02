using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace ExtLog.NET.SingleLineConsoleLogger
{
    [ProviderAlias("SingleLineConsole")]
    public class SingleLineConsoleLoggerProvider : ILoggerProvider // todo: support external scopes
    {
        private readonly IOptionsMonitor<SingleLineConsoleLoggerOptions> _options;
        private readonly ConcurrentDictionary<string, SingleLineConsoleLogger> _loggers = new ConcurrentDictionary<string, SingleLineConsoleLogger>();
        private readonly ActionBlock<ConsoleMessage> _sink = new ActionBlock<ConsoleMessage>(RenderMessage);

        private readonly IDisposable _optionsReloadToken;

        public SingleLineConsoleLoggerProvider(IOptionsMonitor<SingleLineConsoleLoggerOptions> options)
        {
            _options = options;
            _optionsReloadToken = _options.OnChange(ReloadOptions);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, x => new SingleLineConsoleLogger(x, _sink) { Options = _options.CurrentValue });
        }

        public void Dispose()
        {
            _optionsReloadToken?.Dispose();

            _sink.Complete();
            _sink.Completion.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static void RenderMessage(ConsoleMessage msg)
        {
            if (msg.ForegroundColor.HasValue)
            {
                var prevColor = Console.ForegroundColor;
                Console.ForegroundColor = msg.ForegroundColor.Value;
                Console.WriteLine(msg.Message);
                Console.ForegroundColor = prevColor;
            } 
            else
            {
                Console.WriteLine(msg.Message);
            }
        }

        private void ReloadOptions(SingleLineConsoleLoggerOptions options)
        {
            foreach (var logger in _loggers)
            {
                logger.Value.Options = options;
            }
        }
    }
}
