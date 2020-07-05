using System;

namespace LogExCore.SingleLineConsole
{
    /// <summary>
    /// Single Line Console Logger Options
    /// </summary>
    public class SingleLineConsoleLoggerOptions
    {
        /// <summary>
        /// Disable message coloring depending on log level
        /// </summary>
        public bool DisableColors { get; set; }

        /// <summary>
        /// Timestamps format 
        /// </summary>
        public TimestampFormat TimestampFormat { get; set; } = TimestampFormat.TimeMs;

        /// <summary>
        /// Log level code format
        /// </summary>
        public LogLevelFormat LevelFormat { get; set; } = LogLevelFormat.L3;

        /// <summary>
        /// Whether to show full logger name or only class name
        /// </summary>
        public bool FullLoggerName { get; set; }

        /// <summary>
        /// What elements of message should be hidden 
        /// </summary>
        public LogMessageParts[] Hide { get; set; } = Array.Empty<LogMessageParts>();

        internal static SingleLineConsoleLoggerOptions Default { get; } = new SingleLineConsoleLoggerOptions();
    }
}
