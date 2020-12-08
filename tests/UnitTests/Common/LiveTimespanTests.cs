using FluentAssertions;
using Metricaly.Core.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.Common
{
    public class LiveTimespanTests
    {
        private static List<string> ValuesSource =>  new List<string>(LiveTimespan.Values.Keys);

        [Test]
        [TestCaseSource("ValuesSource")]
        public void ShouldParseAllValues(string rawValue)
        {
            var timeSpan = LiveTimespan.GetTimespan(rawValue);

            timeSpan.Should().BePositive();
        }

        [Test]
        public void ShouldThrowExceptionGivenInvalideValue()
        {
            Assert.Throws<Exception>(() => LiveTimespan.GetTimespan("6l"));
        }
    }
}
