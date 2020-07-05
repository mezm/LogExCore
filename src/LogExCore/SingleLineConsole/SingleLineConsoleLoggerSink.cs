using System;
using System.Threading.Tasks.Dataflow;

namespace LogExCore.SingleLineConsole
{
    internal class SingleLineConsoleLoggerSink : ISingleLineConsoleLoggerSink, IDisposable
    {
        private readonly ActionBlock<ConsoleMessage> _sink;

        public SingleLineConsoleLoggerSink() => _sink = new ActionBlock<ConsoleMessage>(RenderMessage);

        public SingleLineConsoleLoggerOptions Options { get; set; } = SingleLineConsoleLoggerOptions.Default;

        public void Push(ConsoleMessage message) => _sink.Post(message);

        public void Dispose()
        {
            _sink.Complete();
            _sink.Completion.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void RenderMessage(ConsoleMessage msg)
        {
            if (!Options.DisableColors)
            {
                var prevColor = Console.ForegroundColor;
                Console.ForegroundColor = msg.ForegroundColor;
                Console.WriteLine(msg.Message);
                Console.ForegroundColor = prevColor;
            }
            else
            {
                Console.WriteLine(msg.Message);
            }
        }
    }
}
