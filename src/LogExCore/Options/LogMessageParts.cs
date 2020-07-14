namespace LogExCore.Options
{
    /// <summary>
    /// Parts of log message
    /// </summary>
    public enum LogMessageParts
    {
        /// <summary>
        /// Timestamp when message was written
        /// </summary>
        Timestamp,
        /// <summary>
        /// Log level of the message
        /// </summary>
        Level,
        /// <summary>
        /// Name of logger (class)
        /// </summary>
        Logger,
        /// <summary>
        /// Message itself 
        /// </summary>
        Message,
        /// <summary>
        /// Exception
        /// </summary>
        Exception
    }
}
