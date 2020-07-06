using System;

namespace LogExCore.SingleLineConsole
{
    internal struct ConsoleMessage
    {
        public ConsoleMessage(string text, ConsoleColor? foregroundColor = null, bool space = true, bool newLine = false)
        {
            Text = text;
            if (space)
            {
                Text += " ";
            }

            ForegroundColor = foregroundColor;

            NewLine = newLine;
        }

        public string Text { get; }

        public ConsoleColor? ForegroundColor { get; }

        public bool NewLine { get; }
    }
}
