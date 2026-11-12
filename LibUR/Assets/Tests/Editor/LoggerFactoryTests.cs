using LibUR.Logging;
using NUnit.Framework;

namespace LibUR.Tests
{
    public class LoggerFactoryTests
    {
        [Test]
        public void Build_DebugDisabled_LogDoesNotThrow()
        {
            ILogger logger = LoggerFactory.Build(isDebugMode: false);
            Assert.DoesNotThrow(() => logger.Log("silent"));
        }

        [Test]
        public void Build_DebugEnabled_LogDoesNotThrow()
        {
            ILogger logger = LoggerFactory.Build(isDebugMode: true);
            Assert.DoesNotThrow(() => logger.Log("visible"));
        }
    }
}
