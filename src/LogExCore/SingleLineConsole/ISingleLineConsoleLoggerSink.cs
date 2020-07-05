namespace LogExCore.SingleLineConsole
{
    internal interface ISingleLineConsoleLoggerSink
    {
        void Push(ConsoleMessage message);
    }
}
