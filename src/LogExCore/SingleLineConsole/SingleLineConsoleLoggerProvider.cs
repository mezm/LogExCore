using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace LogExCore.SingleLineConsole
{
    [ProviderAlias("SingleLineConsole")]
    public partial class SingleLineConsoleLoggerProvider : ILoggerProvider // todo: support external scopes
    {
        private readonly IOptionsMonitor<SingleLineConsoleLoggerOptions> _options;
        private readonly ConcurrentDictionary<string, SingleLineConsoleLogger> _loggers = new ConcurrentDictionary<string, SingleLineConsoleLogger>();
        private readonly SingleLineConsoleLoggerSink _sink = new SingleLineConsoleLoggerSink();

        private readonly IDisposable _optionsReloadToken;

        public SingleLineConsoleLoggerProvider(IOptionsMonitor<SingleLineConsoleLoggerOptions> options)
        {
            _options = options;
            _optionsReloadToken = _options.OnChange(ReloadOptions);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, x => new SingleLineConsoleLogger(x, _sink).WithOptions(_options.CurrentValue));
        }

        public void Dispose() => _optionsReloadToken?.Dispose();

        private void ReloadOptions(SingleLineConsoleLoggerOptions options)
        {
            _sink.Options = options;
            foreach (var logger in _loggers)
            {
                logger.Value.WithOptions(options);
            }
        }
    }
}
