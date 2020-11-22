using NUnit.Framework;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using System.Threading.Tasks;
using Metricaly.Infrastructure.Services;
using FluentAssertions;

namespace Infrastructure.UnitTests.Services
{
    public class MetricRetrieverTests
    {
        private readonly string metricName = "metric";
        private readonly string metricNamespace = "namespace";
        private readonly Guid applicationId = Guid.Empty;
        private readonly string sortedSetKey;

        public MetricRetrieverTests()
        {
            sortedSetKey = $"{applicationId}:{metricNamespace}:{metricName}";
        }


        private IConnectionMultiplexer GetConnectionMultiplexerMockForSortedSetEntries(SortedSetEntry[] sortedSetEntries)
        {
            var databaseMock = new Mock<IDatabase>();
            databaseMock.Setup(x => x.SortedSetRangeByScoreWithScoresAsync(sortedSetKey, 0, 1000, Exclude.None, Order.Ascending, 0, -1, CommandFlags.None))
                .Returns(Task.FromResult(sortedSetEntries));

            var connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
            connectionMultiplexerMock.Setup(x => x.GetDatabase(-1, null))
               .Returns(databaseMock.Object);
            return connectionMultiplexerMock.Object;
        }


        [Test]
        public async Task ShouldReturnParsedSingleMetricsValue()
        {
            var connectionMultiplexer = GetConnectionMultiplexerMockForSortedSetEntries(new SortedSetEntry[] { new SortedSetEntry(new RedisValue("1000 2 10 50 60"), 1000), });

            var metricsRetriever = new MetricsRetriever(connectionMultiplexer);

            var metricValues = await metricsRetriever.QueryAsync(applicationId, metricName, metricNamespace, new Metricaly.Core.Common.TimePeriod { StartTimestamp = 0, EndTimestamp = 1000 });

            metricValues.Should().HaveCount(1);
            metricValues[0].TimeStamp.Should().Be(1000);
            metricValues[0].Count.Should().Be(2);
            metricValues[0].Min.Should().Be(10);
            metricValues[0].Max.Should().Be(50);
            metricValues[0].Sum.Should().Be(60);
        }

        [Test]
        public async Task ShouldReturnParsedFloatingNumberMetricsValue()
        {
            var connectionMultiplexer = GetConnectionMultiplexerMockForSortedSetEntries(new SortedSetEntry[] { new SortedSetEntry(new RedisValue("1000 2 10.33 50.55 60.6666"), 1000), });

            var metricsRetriever = new MetricsRetriever(connectionMultiplexer);

            var metricValues = await metricsRetriever.QueryAsync(applicationId, metricName, metricNamespace, new Metricaly.Core.Common.TimePeriod { StartTimestamp = 0, EndTimestamp = 1000 });

            metricValues.Should().HaveCount(1);
            metricValues[0].TimeStamp.Should().Be(1000);
            metricValues[0].Count.Should().Be(2);
            metricValues[0].Min.Should().Be(10.33);
            metricValues[0].Max.Should().Be(50.55);
            metricValues[0].Sum.Should().Be(60.6666);
        }

        [Test]
        public async Task ShouldReturnMultipleParsedMetricsValue()
        {
            var connectionMultiplexer = GetConnectionMultiplexerMockForSortedSetEntries(new SortedSetEntry[]
            { 
                new SortedSetEntry(new RedisValue("1000 2 10 50 60"), 1000),
                new SortedSetEntry(new RedisValue("1001 2 10 50 60"), 1001),
                new SortedSetEntry(new RedisValue("1002 2 10 50 60"), 1002),
                new SortedSetEntry(new RedisValue("1003 2 10 50 60"), 1003),
            });

            var metricsRetriever = new MetricsRetriever(connectionMultiplexer);

            var metricValues = await metricsRetriever.QueryAsync(applicationId, metricName, metricNamespace, new Metricaly.Core.Common.TimePeriod { StartTimestamp = 0, EndTimestamp = 1000 });

            metricValues.Should().HaveCount(4);

            metricValues[0].TimeStamp.Should().Be(1000);
            metricValues[1].TimeStamp.Should().Be(1001);
            metricValues[2].TimeStamp.Should().Be(1002);
            metricValues[3].TimeStamp.Should().Be(1003);
        }
    }
}
