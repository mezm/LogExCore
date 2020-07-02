using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace ExtLog.NET.SingleLineConsoleLogger
{
    internal class SingleLineConsoleLogger : ILogger
    {
        private readonly string _fullName;
        private readonly string _shortName;
        private readonly ITargetBlock<ConsoleMessage> _sink;

        private static readonly Dictionary<LogLevel, string> LevelMap = new Dictionary<LogLevel, string>
        {
            [LogLevel.None] = "---",
            [LogLevel.Trace] = "TRC",
            [LogLevel.Debug] = "DBG",
            [LogLevel.Information] = "INF",
            [LogLevel.Warning] = "WRN",
            [LogLevel.Error] = "ERR",
            [LogLevel.Critical] = "CRT"
        };

        private static readonly Dictionary<LogLevel, ConsoleColor> ColorMap = new Dictionary<LogLevel, ConsoleColor>
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
            _fullName = name;
            _shortName = GetShortLoggerName(name);
            _sink = sink;
        }

        public SingleLineConsoleLoggerOptions Options { get; set; } = SingleLineConsoleLoggerOptions.Default;

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotSupportedException(); // todo: implement
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var msg = formatter(state, exception);
            var timeFormat = Options.HideMilliseconds ? "HH:mm:ss" : "HH:mm:ss.fff";
            var time = DateTime.Now.ToString(timeFormat);
            var loggerName = Options.ShowFullLoggerName ? _fullName : _shortName;

            var fullMessage = $"{time} {LevelMap[logLevel]} [{loggerName}] {msg}";
            var foregroundColor = Options.DisableColors ? (ConsoleColor?)null : ColorMap[logLevel];
            var consoleMessage = new ConsoleMessage(fullMessage, foregroundColor);

            _sink.Post(consoleMessage);
        }

        private static string GetShortLoggerName(string name)
        {
            var index = name.LastIndexOf('.');
            return index > -1 ? name.Substring(index + 1) : name;
        }
    }
}
