using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

namespace ExtLog.NET.SingleLineConsoleLogger
{
    [ProviderAlias("SingleLineConsole")]
    public class SingleLineConsoleLoggerProvider : ILoggerProvider // todo: support external scopes
    {
        private readonly ConcurrentDictionary<string, SingleLineConsoleLogger> _loggers = new ConcurrentDictionary<string, SingleLineConsoleLogger>();
        private readonly ActionBlock<ConsoleMessage> _sink = new ActionBlock<ConsoleMessage>(RenderMessage);

        public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, x => new SingleLineConsoleLogger(x, _sink));

        public void Dispose()
        {
            _sink.Complete();
            _sink.Completion.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static void RenderMessage(ConsoleMessage msg)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = msg.ForegroundColor;
            Console.WriteLine(msg.Message);
            Console.ForegroundColor = prevColor;
        }
    }
}
