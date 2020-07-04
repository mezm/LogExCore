using System;

namespace LogExCore.SingleLineConsoleLogger
{
    public struct ConsoleMessage
    {
        public ConsoleMessage(string message, ConsoleColor? foregroundColor)
        {
            Message = message;
            ForegroundColor = foregroundColor;
        }

        public string Message { get; }

        public ConsoleColor? ForegroundColor { get; }
    }
}
