using Metricaly.Angular.MetricServices;
using Metricaly.Core;
using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System.Threading.Tasks;

namespace Metricaly.Web
{
    public class CounterService
    {
        private readonly IRedisCacheClient redisCacheClient;
        private readonly ITimeStampProvider timeStampProvider;

        public CounterService(IRedisCacheClient redisCacheClient, ITimeStampProvider timeStampProvider)
        {
            this.redisCacheClient = redisCacheClient;
            this.timeStampProvider = timeStampProvider;
        }

        public async Task<long> Increment(Metric metric, long value)
        {
            var currentTimeStamp = timeStampProvider.GetTimeStamp(Constants.Granularity);
            var redisKey = RedisKeyProvider.GetCounterKey(metric.Id, currentTimeStamp);

            var newCount = await redisCacheClient.Db0.Database.StringIncrementAsync(redisKey, value);

            var added = await redisCacheClient.Db0.SortedSetAddAsync("newlyadded", redisKey, currentTimeStamp);

            return newCount;
        }
    }
}
