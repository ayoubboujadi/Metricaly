using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metricaly.Web
{
    public class MetricRedisService
    {
        private readonly IRedisCacheClient redisCacheClient;

        private const double granularity = 10.0;

        public MetricRedisService(IRedisCacheClient redisCacheClient)
        {
            this.redisCacheClient = redisCacheClient;
        }

        public async Task<long> IncrementCounterByValue(string key, long value)
        {
            return await redisCacheClient.Db0.Database.StringIncrementAsync(key, value);
        }


        public async Task<bool> AddSortedSetValue(string key, string value, long score)
        {
            return await redisCacheClient.Db0.SortedSetAddAsync(key, value, score);
        }



        private long GetCurrentTimestamp()
        {
            var currentSeconds = (int)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
            var timestamp = ((int)Math.Floor(currentSeconds / granularity)) * (int)granularity;
            return timestamp;
        }
    }
}
