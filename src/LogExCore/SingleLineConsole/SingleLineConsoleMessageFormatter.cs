using LogExCore.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogExCore.SingleLineConsole
{
    internal class SingleLineConsoleMessageFormatter
    {
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
        private readonly string _template;
        private readonly bool _fullLoggerName;
        private readonly Dictionary<LogLevel, string> _levelMap;

        public SingleLineConsoleMessageFormatter(SingleLineConsoleLoggerOptions options)
        {
            _timestampFormat = GetDateTimeFormat(options.TimestampFormat);
            _levelMap = GetLogLevelMapping(options.LevelFormat);
            _fullLoggerName = options.FullLoggerName;

            var templateBuilder = new StringBuilder();
            if (!options.Hide.Contains(LogMessageParts.Timestamp)) templateBuilder.Append("{0} ");
            if (!options.Hide.Contains(LogMessageParts.Level)) templateBuilder.Append("{1} ");
            if (!options.Hide.Contains(LogMessageParts.Logger)) templateBuilder.Append("[{2}] ");
            templateBuilder.Append("{3}");
            _template = templateBuilder.ToString();
        }

        public ConsoleMessage FormatFull(LogMessageEntry entry)
        {
            var text = string.Format(_template, entry.Timestamp.ToString(_timestampFormat), _levelMap[entry.Level], GetLoggerName(entry.LoggerName), entry.Message);
            var color = ColorMap[entry.Level];
            return new ConsoleMessage(text, color);
        }

        public IEnumerable<ConsoleMessage> FormatByParts(LogMessageEntry entry)
        {
            // todo: filter hidden fields
            yield return new ConsoleMessage(entry.Timestamp.ToString(_timestampFormat));
            yield return new ConsoleMessage(_levelMap[entry.Level], ColorMap[entry.Level]);
            yield return new ConsoleMessage(GetLoggerName(entry.LoggerName));
            yield return new ConsoleMessage(entry.Message, newLine: true);
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

        private string GetLoggerName(string name)
        {
            if (_fullLoggerName)
            {
                return name;
            }

            var index = name.LastIndexOf('.');
            return index > -1 ? name.Substring(index + 1) : name;
        }
    }
}
