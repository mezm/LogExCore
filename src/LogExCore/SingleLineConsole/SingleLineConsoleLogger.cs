using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogExCore.SingleLineConsole
{
    internal class SingleLineConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly ISingleLineConsoleLoggerSink _sink;

        private static readonly Dictionary<LogLevel, ConsoleColor> ColorMap = new Dictionary<LogLevel, ConsoleColor>
        {
            [LogLevel.None] = ConsoleColor.Cyan,
            [LogLevel.Trace] = ConsoleColor.DarkCyan,
            [LogLevel.Debug] = ConsoleColor.DarkGray,
            [LogLevel.Information] = ConsoleColor.White,
            [LogLevel.Warning] = ConsoleColor.DarkYellow,
            [LogLevel.Error] = ConsoleColor.DarkRed,
            [LogLevel.Critical] = ConsoleColor.Red
        };

        private Formatter _formatter;
        private IExternalScopeProvider _scopeProvider;

        public SingleLineConsoleLogger(string name, ISingleLineConsoleLoggerSink sink, SingleLineConsoleLoggerOptions options, IExternalScopeProvider scopeProvider)
        {
            _name = name;
            _sink = sink;
            _scopeProvider = scopeProvider;
            WithOptions(SingleLineConsoleLoggerOptions.Default);
        }

        public void WithOptions(SingleLineConsoleLoggerOptions options) => _formatter = new Formatter(_name, options);

        public void WithScopeProvider(IExternalScopeProvider scopeProvider) => _scopeProvider = scopeProvider;

        public IDisposable BeginScope<TState>(TState state) => _scopeProvider?.Push(state);

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var msg = formatter(state, exception);
            var fullMessage = _formatter.Format(DateTime.Now, logLevel, msg);
            var foregroundColor = ColorMap[logLevel];
            var consoleMessage = new ConsoleMessage(fullMessage, foregroundColor);

            _sink.Push(consoleMessage);
        }

        private class Formatter
        {
            private static readonly Dictionary<LogLevel, string> OneLeterLevelMap = new Dictionary<LogLevel, string>
            {
                [LogLevel.None] = "N",
                [LogLevel.Trace] = "T",
                [LogLevel.Debug] = "D",
                [LogLevel.Information] = "I",
                [LogLevel.Warning] = "W",
                [LogLevel.Error] = "E",
                [LogLevel.Critical] = "C"
            };

            private static readonly Dictionary<LogLevel, string> ThreeLeterLevelMap = new Dictionary<LogLevel, string>
            {
                [LogLevel.None] = "NON",
                [LogLevel.Trace] = "TRC",
                [LogLevel.Debug] = "DBG",
                [LogLevel.Information] = "INF",
                [LogLevel.Warning] = "WRN",
                [LogLevel.Error] = "ERR",
                [LogLevel.Critical] = "CRT"
            };

            private static readonly Dictionary<LogLevel, string> FullLevelMap = new Dictionary<LogLevel, string>
            {
                [LogLevel.None] = "NONE ",
                [LogLevel.Trace] = "TRACE",
                [LogLevel.Debug] = "DEBUG",
                [LogLevel.Information] = "INFO ",
                [LogLevel.Warning] = "WARN ",
                [LogLevel.Error] = "ERROR",
                [LogLevel.Critical] = "CRIT "
            };

            private readonly string _timestampFormat;
            private readonly string _loggerName;
            private readonly string _template;
            private readonly Dictionary<LogLevel, string> _levelMap;

            public Formatter(string name, SingleLineConsoleLoggerOptions options)
            {
                _loggerName = options.FullLoggerName ? name : GetShortLoggerName(name);
                _timestampFormat = GetDateTimeFormat(options.TimestampFormat);
                _levelMap = GetLogLevelMapping(options.LevelFormat);

                var templateBuilder = new StringBuilder();
                if (!options.Hide.Contains(LogMessageParts.Timestamp)) templateBuilder.Append("{0} ");
                if (!options.Hide.Contains(LogMessageParts.Level)) templateBuilder.Append("{1} ");
                if (!options.Hide.Contains(LogMessageParts.Logger)) templateBuilder.Append("[{2}] ");
                templateBuilder.Append("{3}");
                _template = templateBuilder.ToString();
            }

            public string Format(DateTime timestamp, LogLevel level, string message)
            {
                return string.Format(_template, timestamp.ToString(_timestampFormat), _levelMap[level], _loggerName, message);
            }

            private static string GetDateTimeFormat(TimestampFormat format)
            {
                switch (format)
                {
                    case TimestampFormat.DateTimeMs:
                        return "yyyy-MM-dd HH:mm:ss.fff";
                    case TimestampFormat.TimeMs:
                        return "HH:mm:ss.fff";
                    case TimestampFormat.Time:
                        return "HH:mm:ss";
                }

                return "";
            }

            private static Dictionary<LogLevel, string> GetLogLevelMapping(LogLevelFormat format)
            {
                switch (format)
                {
                    case LogLevelFormat.L1:
                        return OneLeterLevelMap;
                    case LogLevelFormat.L3:
                        return ThreeLeterLevelMap;
                    case LogLevelFormat.Full:
                        return FullLevelMap;
                }

                return null;
            }

            private static string GetShortLoggerName(string name)
            {
                var index = name.LastIndexOf('.');
                return index > -1 ? name.Substring(index + 1) : name;
            }
        }
    }
}
