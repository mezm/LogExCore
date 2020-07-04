namespace LogExCore.SingleLineConsoleLogger
{
    public class SingleLineConsoleLoggerOptions
    {
        public bool DisableColors { get; set; }

        public bool HideMilliseconds { get; set; }

        public bool HideLoggerName { get; set; }

        public bool ShowFullLoggerName { get; set; }

        public static SingleLineConsoleLoggerOptions Default { get; } = new SingleLineConsoleLoggerOptions();
    }
}
