using FactoryBot;
using FluentAssertions;
using LogExCore.Options;
using LogExCore.SingleLineConsole;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogExCore.Tests.SingleLineConsole
{
    [TestFixture]
    public class SingleLineConsoleMessageFormatterTest
    {
        [SetUp]
        public void Init()
        {
            Bot.Define(
                x => new LogMessageEntry(
                    x.Dates.BeforeNow(), 
                    LogLevel.Information, 
                    "LogExCore.Tests.SingleLineConsole.SingleLineConsoleMessageFormatterTest", 
                    x.Strings.Any()));
        }

        [TestCase(TimestampFormat.Time, @"\d{2}:\d{2}:\d{2}")]
        [TestCase(TimestampFormat.DateTimeMs, @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}\.\d{3}")]
        [TestCase(TimestampFormat.TimeMs, @"\d{2}:\d{2}:\d{2}\.\d{3}")]
        public void FormatMessageParts_FormatTimestamp_TimestampIsFormattedCorrectly(TimestampFormat format, string pattern)
        {
            var formatter = CreateFormatter(x => x.TimestampFormat = format);

            var messages = formatter.FormatMessageParts(Bot.Build<LogMessageEntry>());

            messages.First().Text.Should().MatchRegex(pattern);
        }

        [TestCase(LogLevelFormat.L1, "I")]
        [TestCase(LogLevelFormat.L3, "INF")]
        [TestCase(LogLevelFormat.Full, "INFO")]
        public void FormatMessageParts_FormatLevel_LevelIsFormattedCorrectly(LogLevelFormat format, string level)
        {
            var formatter = CreateFormatter(x => x.LevelFormat = format);

            var messages = formatter.FormatMessageParts(Bot.Build<LogMessageEntry>());

            messages.Skip(1).First().Text.TrimEnd().Should().Be(level);
        }

        [Test]
        public void FormatMessageParts_FullLoggerName_LoggerNameIsFull()
        {
            var formatter = CreateFormatter(x => x.FullLoggerName = true);

            var messages = formatter.FormatMessageParts(Bot.Build<LogMessageEntry>());

            messages.Skip(2).First().Text.TrimEnd().Should().Be("[LogExCore.Tests.SingleLineConsole.SingleLineConsoleMessageFormatterTest]");
        }

        [Test]
        public void FormatMessageParts_ShortLoggerName_LoggerNameIsShort()
        {
            var formatter = CreateFormatter(x => x.FullLoggerName = false);

            var messages = formatter.FormatMessageParts(Bot.Build<LogMessageEntry>());

            messages.Skip(2).First().Text.TrimEnd().Should().Be("[SingleLineConsoleMessageFormatterTest]");
        }

        [Test]
        public void FormatMessageParts_DisableColors_AllMessagesNotColored()
        {
            var formatter = CreateFormatter(x => x.DisableColors = true);

            var messages = formatter.FormatMessageParts(Bot.Build<LogMessageEntry>());

            messages.Should().OnlyContain(x => !x.ForegroundColor.HasValue);
        }

        [Test]
        public void FormatMessageParts_EnableColors_LevelAndMessageAreColored()
        {
            var formatter = CreateFormatter(x => x.DisableColors = false);

            var messages = formatter.FormatMessageParts(Bot.Build<LogMessageEntry>());

            messages.Skip(1).First().ForegroundColor.Should().Be(ConsoleColor.White);
            messages.Skip(3).First().ForegroundColor.Should().Be(ConsoleColor.White);
        }

        [TestCase(LogMessageParts.Timestamp, @"^\d+")]
        [TestCase(LogMessageParts.Level, "^INF")]
        [TestCase(LogMessageParts.Logger, "SingleLineConsoleMessageFormatterTest]")]
        public void FormatMessageParts_HidePart_PartSkipped(LogMessageParts part, string pattern)
        {
            var formatter = CreateFormatter(x => x.Hide = new LogMessageParts[] { part });

            var messages = formatter.FormatMessageParts(Bot.Build<LogMessageEntry>());

            messages.Should().HaveCount(3).And.OnlyContain(x => !Regex.IsMatch(x.Text, pattern));
        }

        [Test]
        public void FormatMessageParts_HideMessage_MessageNotSkipped()
        {
            var formatter = CreateFormatter(x => x.Hide = new LogMessageParts[] { LogMessageParts.Message });

            var messages = formatter.FormatMessageParts(Bot.Build<LogMessageEntry>());

            messages.Should().HaveCount(4);
        }

        private static SingleLineConsoleMessageFormatter CreateFormatter(Action<SingleLineConsoleLoggerOptions> configAction)
        {
            var config = new SingleLineConsoleLoggerOptions();
            configAction(config);
            return new SingleLineConsoleMessageFormatter(config);
        }
    }
}
