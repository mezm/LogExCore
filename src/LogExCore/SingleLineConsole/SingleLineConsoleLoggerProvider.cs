using LogExCore.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace LogExCore.SingleLineConsole
{
    [ProviderAlias("SingleLineConsole")]
    public partial class SingleLineConsoleLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly IOptionsMonitor<SingleLineConsoleLoggerOptions> _options;
        private readonly ConcurrentDictionary<string, SingleLineConsoleLogger> _loggers = new ConcurrentDictionary<string, SingleLineConsoleLogger>();
        private readonly SingleLineConsoleLoggerSink _sink;

        private readonly IDisposable _optionsReloadToken;

        private IExternalScopeProvider _scopeProvider = DummyExternalScopeProvider.Instance;

        public SingleLineConsoleLoggerProvider(IOptionsMonitor<SingleLineConsoleLoggerOptions> options)
        {
            _options = options;
            _optionsReloadToken = _options.OnChange(ReloadOptions);
            _sink = new SingleLineConsoleLoggerSink(_options.CurrentValue);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, x => new SingleLineConsoleLogger(x, _sink, _options.CurrentValue, _scopeProvider));
        }

        public void Dispose() => _optionsReloadToken?.Dispose();

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
            _loggers.Values.ToList().ForEach(x => x.WithScopeProvider(scopeProvider));
        }

        private void ReloadOptions(SingleLineConsoleLoggerOptions options) => _sink.WithOptions(options);
    }
}
