using FluentAssertions;
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
        private ISingleLineConsoleLoggerSink _sink;

        [SetUp]
        public void Init()
        {
            _sink = Substitute.For<ISingleLineConsoleLoggerSink>();
            _logger = new SingleLineConsoleLogger(GetType().FullName, _sink, SingleLineConsoleLoggerOptions.Default, DummyExternalScopeProvider.Instance);
        }

        [Test]
        public void WriteMessage_NoConditions_FormattedMessagePushedToSink()
        {
            _logger.WithOptions(new SingleLineConsoleLoggerOptions { FullLoggerName = false, LevelFormat = LogLevelFormat.L1, TimestampFormat = TimestampFormat.Time });

            _logger.LogInformation("test message with {int}", 12);

            _sink.ReceivedWithAnyArgs().Push(default);
            var msg = (ConsoleMessage)_sink.ReceivedCalls().First().GetArguments()[0];

            msg.Message.Should().MatchRegex(@"^\d{2}:\d{2}:\d{2}");
            msg.Message.Substring(9).Should().Be("I [SingleLineConsoleLoggerTest] test message with 12");
        }

        [TestCase(LogLevel.None, ConsoleColor.Cyan)]
        [TestCase(LogLevel.Trace, ConsoleColor.DarkCyan)]
        [TestCase(LogLevel.Debug, ConsoleColor.DarkGray)]
        [TestCase(LogLevel.Information, ConsoleColor.White)]
        [TestCase(LogLevel.Warning, ConsoleColor.DarkYellow)]
        [TestCase(LogLevel.Error, ConsoleColor.DarkRed)]
        [TestCase(LogLevel.Critical, ConsoleColor.Red)]
        public void WriteMessage_WithLevel_ColorShouldReflectLevel(LogLevel level, ConsoleColor color)
        {
            _logger.Log(level, "any");

            _sink.Received().Push(Arg.Is<ConsoleMessage>(x => x.ForegroundColor == color));
        }

        [TestCase(TimestampFormat.DateTimeMs, @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}\.\d{3}")]
        [TestCase(TimestampFormat.TimeMs, @"\d{2}:\d{2}:\d{2}\.\d{3}")]
        [TestCase(TimestampFormat.Time, @"\d{2}:\d{2}:\d{2}")]
        public void WriteMessage_WithTimestamp_MessageShouldContainCorrectDateTime(TimestampFormat format, string dateTimeRx)
        {
            _logger.WithOptions(new SingleLineConsoleLoggerOptions { TimestampFormat = format });

            _logger.LogDebug("any");

            _sink.ReceivedWithAnyArgs().Push(default);
            var msg = (ConsoleMessage)_sink.ReceivedCalls().First().GetArguments()[0];
            msg.Message.Should().MatchRegex(dateTimeRx);
        }

        [TestCase(LogLevelFormat.L1, LogLevel.None, "N")]
        [TestCase(LogLevelFormat.L1, LogLevel.Trace, "T")]
        [TestCase(LogLevelFormat.L1, LogLevel.Debug, "D")]
        [TestCase(LogLevelFormat.L1, LogLevel.Information, "I")]
        [TestCase(LogLevelFormat.L1, LogLevel.Warning, "W")]
        [TestCase(LogLevelFormat.L1, LogLevel.Error, "E")]
        [TestCase(LogLevelFormat.L1, LogLevel.Critical, "C")]
        [TestCase(LogLevelFormat.L3, LogLevel.None, "NON")]
        [TestCase(LogLevelFormat.L3, LogLevel.Trace, "TRC")]
        [TestCase(LogLevelFormat.L3, LogLevel.Debug, "DBG")]
        [TestCase(LogLevelFormat.L3, LogLevel.Information, "INF")]
        [TestCase(LogLevelFormat.L3, LogLevel.Warning, "WRN")]
        [TestCase(LogLevelFormat.L3, LogLevel.Error, "ERR")]
        [TestCase(LogLevelFormat.L3, LogLevel.Critical, "CRT")]
        [TestCase(LogLevelFormat.Full, LogLevel.None, "NONE ")]
        [TestCase(LogLevelFormat.Full, LogLevel.Trace, "TRACE")]
        [TestCase(LogLevelFormat.Full, LogLevel.Debug, "DEBUG")]
        [TestCase(LogLevelFormat.Full, LogLevel.Information, "INFO ")]
        [TestCase(LogLevelFormat.Full, LogLevel.Warning, "WARN ")]
        [TestCase(LogLevelFormat.Full, LogLevel.Error, "ERROR")]
        [TestCase(LogLevelFormat.Full, LogLevel.Critical, "CRIT ")]
        public void WriteMessage_WithTimestamp_MessageShouldContainCorrectDateTime(LogLevelFormat format, LogLevel level, string levelStr)
        {
            _logger.WithOptions(new SingleLineConsoleLoggerOptions { LevelFormat = format });

            _logger.Log(level, "any");

            _sink.Received().Push(Arg.Is<ConsoleMessage>(x => x.Message.Contains($" {levelStr} ")));
        }

        [Test]
        public void WriteMessage_WithFullLoggerName_MessageShouldHaveFullName()
        {
            _logger.WithOptions(new SingleLineConsoleLoggerOptions { FullLoggerName = true });

            _logger.LogDebug("any");

            _sink.Received().Push(Arg.Is<ConsoleMessage>(x => x.Message.Contains(" [LogExCore.Tests.SingleLineConsole.SingleLineConsoleLoggerTest] ")));
        }

        [Test]
        public void WriteMessage_WithShortLoggerName_MessageShouldHaveShortName()
        {
            _logger.WithOptions(new SingleLineConsoleLoggerOptions { FullLoggerName = false });

            _logger.LogDebug("any");

            _sink.Received().Push(Arg.Is<ConsoleMessage>(x => x.Message.Contains(" [SingleLineConsoleLoggerTest] ")));
        }

        [Test]
        public void WriteMessage_HideTimestamp_MessageShouldNotHaveTimestamp()
        {
            _logger.WithOptions(new SingleLineConsoleLoggerOptions { Hide = new[] { LogMessageParts.Timestamp }, LevelFormat = LogLevelFormat.Full });

            _logger.LogDebug("any");

            _sink.Received().Push(Arg.Is<ConsoleMessage>(x => x.Message.StartsWith("DEBUG")));
        }

        [Test]
        public void WriteMessage_HideLevel_MessageShouldNotHaveLevel()
        {
            _logger.WithOptions(new SingleLineConsoleLoggerOptions { Hide = new[] { LogMessageParts.Level }, LevelFormat = LogLevelFormat.Full });

            _logger.LogDebug("any");

            _sink.Received().Push(Arg.Is<ConsoleMessage>(x => !x.Message.Contains("DEBUG")));
        }

        [Test]
        public void WriteMessage_HideLogger_MessageShouldNotHaveLogger()
        {
            _logger.WithOptions(new SingleLineConsoleLoggerOptions { Hide = new[] { LogMessageParts.Logger } });

            _logger.LogDebug("any");

            _sink.Received().Push(Arg.Is<ConsoleMessage>(x => !x.Message.Contains("SingleLineConsoleLoggerTest")));
        }

        [Test]
        public void WriteMessage_HideMessage_MessageShouldBeFull()
        {
            _logger.WithOptions(new SingleLineConsoleLoggerOptions { Hide = new[] { LogMessageParts.Message } });

            _logger.LogDebug("any log message");

            _sink.Received().Push(Arg.Is<ConsoleMessage>(x => x.Message.Contains("any log message")));
        }
    }
}
