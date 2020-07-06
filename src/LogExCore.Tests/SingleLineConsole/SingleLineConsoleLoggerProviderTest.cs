using FluentAssertions;
using LogExCore.Options;
using LogExCore.SingleLineConsole;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

namespace LogExCore.Tests.SingleLineConsole
{
    [TestFixture]
    public class SingleLineConsoleLoggerProviderTest
    {
        private SingleLineConsoleLoggerProvider _provider;
        private IOptionsMonitor<SingleLineConsoleLoggerOptions> _optionsMonitor;

        [SetUp]
        public void Init()
        {
            _optionsMonitor = Substitute.For<IOptionsMonitor<SingleLineConsoleLoggerOptions>>();
            _optionsMonitor.CurrentValue.Returns(SingleLineConsoleLoggerOptions.Default);

            _provider = new SingleLineConsoleLoggerProvider(_optionsMonitor);
        }

        [Test]
        public void CreateLogger_CallTwiceTheSame_SingleInstanceCreated()
        {
            var logger1 = _provider.CreateLogger("a");
            var logger2 = _provider.CreateLogger("b");
            var logger3 = _provider.CreateLogger("a");

            logger1.Should().BeSameAs(logger3);
            logger1.Should().NotBeSameAs(logger2);
        }

        [Test, Ignore("No way to test properly")]
        public void OnOptionsUpdate_NoCondition_AllLoggersAndSinkHasUpdatedOptions()
        {
            _optionsMonitor.ReceivedWithAnyArgs().OnChange(default);
            // todo: finish the test
        }
    }
}
