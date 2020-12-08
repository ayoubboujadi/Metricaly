using NUnit.Framework;
using FluentAssertions;
using Metricaly.Core.Common.Utils;

namespace Infrastructure.UnitTests.Common.Utils
{
    public class TimeStampProviderTests
    {
        [Test]
        public void ShouldReturnTimestampInMinutesPrecision()
        {
            Assert.DoesNotThrow(() => TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Minutes));
        }

        [Test]
        public void ShouldReturnTimestampInSecondsPrecision()
        {
            Assert.DoesNotThrow(() => TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Seconds));
        }

        [Test]
        public void ShouldReturnTimestampInMillisecondsPrecision()
        {
            Assert.DoesNotThrow(() => TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Milliseconds));
        }

        [Test]
        public void ShouldReturnTimestampInMicrosecondsPrecision()
        {
            Assert.DoesNotThrow(() => TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Microseconds));
        }

        [Test]
        public void ShouldReturnTimestampInNanosecondsPrecision()
        {
            Assert.DoesNotThrow(() => TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Nanoseconds));
        }

        [Test]
        [TestCase(2.0)]
        [TestCase(5.0)]
        [TestCase(10.0)]
        [TestCase(100.0)]
        [TestCase(300.0)]
        [TestCase(3600.0)]
        public void ShouldReturnTimestampWithSecondsGranularity(double granularity)
        {
            var timestamp = TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Seconds, granularity);

            (timestamp / granularity).Should().Equals(0);
        }
    }
}