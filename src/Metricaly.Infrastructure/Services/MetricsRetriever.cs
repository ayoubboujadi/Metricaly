using Metricaly.Core.Common;
using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Services
{
    public class MetricsRetriever : IMetricsRetriever
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;

        public MetricsRetriever(IConnectionMultiplexer connectionMultiplexer)
        {
            this.connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<MetricValue[]> QueryAsync(Metric metric, TimePeriod timePeriod)
        {
            return await QueryAsync(metric.ApplicationId, metric.Name, metric.Namespace, timePeriod);
        }

        public async Task<MetricValue[]> QueryAsync(Guid applicationId, string metricName, string metricNamespace, TimePeriod timePeriod)
        {
            var redisDb = connectionMultiplexer.GetDatabase();

            var metricSortedSetKey = $"{applicationId}:{metricNamespace}:{metricName}";

            var result = await redisDb.SortedSetRangeByScoreWithScoresAsync(metricSortedSetKey, timePeriod.StartTimestamp, timePeriod.EndTimestamp);

            var values = new MetricValue[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var data = result[i].Element.ToString().Trim('"').Split(' ');

                values[i] = new MetricValue
                {
                    TimeStamp = (long)result[i].Score,
                    Count = double.Parse(data[1]),
                    Min = double.Parse(data[2]),
                    Max = double.Parse(data[3]),
                    Sum = double.Parse(data[4])
                };
            }

            return values;
        }
    }
}
