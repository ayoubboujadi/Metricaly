using FluentAssertions;
using Metricaly.Core.Common.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.Common.Utils
{
    public class TimePeriodUtilsTests
    {
        [Test]
        public void ShouldParseTimePeriodGivenStartTime()
        {
            var timePeriod = TimePeriodUtils.Parse(10000, null, null);

            timePeriod.Should().NotBeNull();
            timePeriod.StartTimestamp.Should().Be(10000);
            timePeriod.EndTimestamp.Should().BeGreaterThan(10000);
        }

        [Test]
        public void ShouldParseTimePeriodGivenStartAndEndTime()
        {
            var timePeriod = TimePeriodUtils.Parse(10000, 20000, null);

            timePeriod.Should().NotBeNull();
            timePeriod.StartTimestamp.Should().Be(10000);
            timePeriod.EndTimestamp.Should().Be(20000);
        }

        [Test]
        public void ShouldParseTimePeriodGivenTimeSpan()
        {
            var timePeriod = TimePeriodUtils.Parse(null, null, "5m");

            timePeriod.Should().NotBeNull();
            (timePeriod.EndTimestamp - timePeriod.StartTimestamp).Should().Be(300);
        }

        [Test]
        public void ShouldParseAndPrioritizeTimePeriodGivenTimeSpanAndStartAndEndTimes()
        {
            var timePeriod = TimePeriodUtils.Parse(10000, 20000, "5m");

            timePeriod.Should().NotBeNull();
            (timePeriod.EndTimestamp - timePeriod.StartTimestamp).Should().Be(300);
        }

        [Test]
        public void ShouldThrowExceptionIfNoStartTimeOrLiveSpanProvided()
        {
            Assert.Throws<Exception>(() => TimePeriodUtils.Parse(null, 20000, null));
        }
    }
}
