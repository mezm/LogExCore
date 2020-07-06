namespace LogExCore
{
    internal interface ILoggerSink
    {
        void Push(LogMessageEntry message);
    }
}
