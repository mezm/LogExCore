using LogExCore.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace LogExCore.SingleLineConsole
{
    internal class SingleLineConsoleLoggerSink : ILoggerSink, IDisposable
    {
        private readonly ActionBlock<ConsoleMessage> _renderer;
        private readonly TransformManyBlock<LogMessageEntry, ConsoleMessage> _sink;

        private SingleLineConsoleMessageFormatter _formatter;

        public SingleLineConsoleLoggerSink(SingleLineConsoleLoggerOptions options)
        {
            _renderer = new ActionBlock<ConsoleMessage>(RenderMessage);
            _sink = new TransformManyBlock<LogMessageEntry, ConsoleMessage>(ProcessMessage, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 100 }); // todo: think about magic number

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            _sink.LinkTo(_renderer, linkOptions);

            WithOptions(options);
        }

        public void WithOptions(SingleLineConsoleLoggerOptions options) => _formatter = new SingleLineConsoleMessageFormatter(options);

        public void Push(LogMessageEntry message) => _sink.Post(message);

        public void Dispose()
        {
            _sink.Complete();
            _renderer.Completion.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private IEnumerable<ConsoleMessage> ProcessMessage(LogMessageEntry entry) => _formatter.FormatMessageParts(entry);

        private void RenderMessage(ConsoleMessage msg)
        {
            if (msg.ForegroundColor.HasValue && Console.ForegroundColor != msg.ForegroundColor.Value)
            {
                var prevColor = Console.ForegroundColor;
                Console.ForegroundColor = msg.ForegroundColor.Value;
                Console.Write(msg.Text);
                Console.ForegroundColor = prevColor;
            }
            else
            {
                Console.Write(msg.Text); 
            }

            if (msg.NewLine)
            {
                Console.WriteLine();
            }
        }
    }
}
