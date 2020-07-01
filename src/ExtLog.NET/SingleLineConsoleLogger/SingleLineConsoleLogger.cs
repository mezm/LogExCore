using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace ExtLog.NET.SingleLineConsoleLogger
{
    public class SingleLineConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly ITargetBlock<ConsoleMessage> _sink;

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

        private readonly Dictionary<LogLevel, ConsoleColor> _colorMap = new Dictionary<LogLevel, ConsoleColor>
        {
            [LogLevel.None] = ConsoleColor.DarkYellow,
            [LogLevel.Trace] = ConsoleColor.Gray,
            [LogLevel.Debug] = ConsoleColor.DarkGray,
            [LogLevel.Information] = ConsoleColor.White,
            [LogLevel.Warning] = ConsoleColor.Yellow,
            [LogLevel.Error] = ConsoleColor.Red,
            [LogLevel.Critical] = ConsoleColor.DarkRed
        };

        public SingleLineConsoleLogger(string name, ITargetBlock<ConsoleMessage> sink)
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
            var fullMessage = $"[{DateTime.Now:hh:mm:ss.FFF}] {_levelMap[logLevel]} - {_name}: {msg}";

            var consoleMessage = new ConsoleMessage(fullMessage, _colorMap[logLevel]);
            _sink.Post(consoleMessage);
        }
    }
}
