using LogExCore.Options;
using Microsoft.Extensions.Logging;
using System;

namespace LogExCore.SingleLineConsole
{
    internal class SingleLineConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly ILoggerSink _sink;

        private IExternalScopeProvider _scopeProvider;

        public SingleLineConsoleLogger(string name, ILoggerSink sink, SingleLineConsoleLoggerOptions options, IExternalScopeProvider scopeProvider)
        {
            _name = name;
            _sink = sink;
            _scopeProvider = scopeProvider;
        }

        public void WithScopeProvider(IExternalScopeProvider scopeProvider) => _scopeProvider = scopeProvider;

        public IDisposable BeginScope<TState>(TState state) => _scopeProvider?.Push(state);

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);
            var entry = new LogMessageEntry(DateTime.Now, logLevel, _name, message);
            _sink.Push(entry);
        }
    }
}
