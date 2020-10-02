using Metricaly.Angular.MetricServices;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metricaly.Web
{
    public class MetricsAggregator
    {
        private readonly IRedisCacheClient redisCacheClient;

        public MetricsAggregator(IRedisCacheClient redisCacheClient)
        {
            this.redisCacheClient = redisCacheClient;
        }

        public async Task<int> Aggregate()
        {
            // 1. Read newly added values from the Sorted Set
            // 2. Go through these values and read the related counter for each one
            // 3. Insert the counter's value to the sorted set for the related metric
            // 4. Delete the counter
            // 5. Delete the newly added values

            var currentSeconds1 = (int)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
            var currentSeconds = (((int)Math.Floor(currentSeconds1 / 10.0)) * 10) - 10;

            // Read newly added values
            var sortedSetEntries = await redisCacheClient.Db0.Database.SortedSetRangeByScoreWithScoresAsync(RedisKeyProvider.NewlyAddedCountersSortedSetName, double.NegativeInfinity, currentSeconds);

            var countersToRemove = new List<RedisKey>();

            foreach (var sortedSetEntry in sortedSetEntries)
            {
                // metricId:c:timestamp
                string key = sortedSetEntry.Element.ToString().Trim('"');
                var timestamp = sortedSetEntry.Score;

                // Get the counter's value
                var value = await redisCacheClient.Db0.GetAsync<long>(key);

                // Insert the counter's value in the related metric's table
                var metricId = key.Split(new string[] { ":c:" }, StringSplitOptions.None)[0];
                var targetKey = RedisKeyProvider.GetMetricSortedSetKey(metricId);
                var added = await redisCacheClient.Db0.SortedSetAddAsync(targetKey, value + ":" + timestamp, timestamp);
                if (added)
                {
                    countersToRemove.Add(key);
                }
            }

            if (countersToRemove.Count != 0)
            {
                // Delete the counters
                await redisCacheClient.Db0.Database.KeyDeleteAsync(countersToRemove.ToArray());
                // Remove newly added values
                await redisCacheClient.Db0.Database.SortedSetRemoveRangeByScoreAsync(RedisKeyProvider.NewlyAddedCountersSortedSetName, 0, currentSeconds);
            }

            return countersToRemove.Count;
        }
    }
}
