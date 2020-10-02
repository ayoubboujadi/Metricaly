using Metricaly.Angular.MetricServices;
using Metricaly.Core.Entities;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metricaly.Web
{
    public class TimePeriod
    {
        public long StartTimestamp { get; set; }
        public long EndTimestamp { get; set; }
    }

    public class MetricValue
    {
        public long TimeStamp { get; set; }
        public long? Value { get; set; }
    }

    public class MetricsRetriever
    {
        private readonly IRedisCacheClient redisCacheClient;

        public MetricsRetriever(IRedisCacheClient redisCacheClient)
        {
            this.redisCacheClient = redisCacheClient;
        }

        public async Task<MetricValue[]> GetMetricValues(Metric metric, TimePeriod timePeriod)
        {
            var metricSortedSetKey = RedisKeyProvider.GetMetricSortedSetKey(metric);

            var result = await redisCacheClient.Db0.Database.SortedSetRangeByScoreAsync(metricSortedSetKey, timePeriod.StartTimestamp, timePeriod.EndTimestamp);

            var values = new MetricValue[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var data = result[i].ToString().Trim('"').Split(':');
                long value = long.Parse(data[0]);
                long timespan = long.Parse(data[1]);

                values[i] = new MetricValue
                {
                    TimeStamp = timespan,
                    Value = value
                };
            }

            return values;
        }


    }
}
