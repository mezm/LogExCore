using FluentAssertions;
using LogExCore.Options;
using LogExCore.SingleLineConsole;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;

namespace LogExCore.Tests.SingleLineConsole
{
    [TestFixture]
    public class SingleLineConsoleLoggerTest
    {
        private SingleLineConsoleLogger _logger;
        private ILoggerSink _sink;

        [SetUp]
        public void Init()
        {
            _sink = Substitute.For<ILoggerSink>();
            _logger = new SingleLineConsoleLogger(GetType().FullName, _sink, SingleLineConsoleLoggerOptions.Default, DummyExternalScopeProvider.Instance);
        }

        [Test]
        public void WriteMessage_Info_MessagePushedToSink()
        {
            _logger.LogInformation("test message with {int}", 12);

            _sink.ReceivedWithAnyArgs().Push(default);
            var msg = (LogMessageEntry)_sink.ReceivedCalls().First().GetArguments()[0];

            msg.Timestamp.Should().BeCloseTo(DateTime.Now);
            msg.Level.Should().Be(LogLevel.Information);
            msg.LoggerName.Should().Be("LogExCore.Tests.SingleLineConsole.SingleLineConsoleLoggerTest");
            msg.Message.Should().Be("test message with 12");
            msg.Exception.Should().BeNull();
        }

        [Test]
        public void WriteMessasge_WithException_MessagePushedToSink()
        {
            var ex = new InvalidOperationException("TEST");
            _logger.LogWarning(ex, "Exception is here");

            _sink.ReceivedWithAnyArgs().Push(default);
            var msg = (LogMessageEntry)_sink.ReceivedCalls().First().GetArguments()[0];

            msg.Timestamp.Should().BeCloseTo(DateTime.Now);
            msg.Level.Should().Be(LogLevel.Warning);
            msg.LoggerName.Should().Be("LogExCore.Tests.SingleLineConsole.SingleLineConsoleLoggerTest");
            msg.Message.Should().Be("Exception is here");
            msg.Exception.Should().BeSameAs(ex);
        }

        [Test]
        public void BeginScope_WithState_PushNewStateToScopeProvider()
        {
            var scopeProvider = Substitute.For<IExternalScopeProvider>();
            _logger.WithScopeProvider(scopeProvider);

            _logger.BeginScope(456);

            scopeProvider.Received().Push(456);
        }
    }
}
