using System;

namespace LogExCore.SingleLineConsoleLogger
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
