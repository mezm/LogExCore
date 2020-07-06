using Microsoft.Extensions.Logging;
using System;

namespace LogExCore
{
    public struct LogMessageEntry
    {
        public LogMessageEntry(DateTime timestamp, LogLevel level, string loggerName, string message)
        {
            Timestamp = timestamp;
            Level = level;
            LoggerName = loggerName;
            Message = message;
        }

        public DateTime Timestamp { get; }

        public LogLevel Level { get; }

        public string LoggerName { get; }

        public string Message { get; }
    }
}
