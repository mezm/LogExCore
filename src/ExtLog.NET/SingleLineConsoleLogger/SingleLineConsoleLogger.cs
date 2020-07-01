using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace ExtLog.NET.SingleLineConsoleLogger
{
    // todo: implement coloring
    public class SingleLineConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly ITargetBlock<string> _sink;

        private readonly Dictionary<LogLevel, string> _levelMap = new Dictionary<LogLevel, string>
        {
            [LogLevel.None] = "---",
            [LogLevel.Trace] = "TRC",
            [LogLevel.Debug] = "DBG",
            [LogLevel.Information] = "INF",
            [LogLevel.Warning] = "WRN",
            [LogLevel.Error] = "ERR",
            [LogLevel.Critical] = "CRT"
        };

        public SingleLineConsoleLogger(string name, ITargetBlock<string> sink)
        {
            _name = name;
            _sink = sink;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotSupportedException(); // todo: implement
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var msg = formatter(state, exception);
            var fullMessage = $"[{DateTime.Now}] {_levelMap[logLevel]} - {_name}: {msg}";
            _sink.Post(fullMessage);
        }
    }
}
