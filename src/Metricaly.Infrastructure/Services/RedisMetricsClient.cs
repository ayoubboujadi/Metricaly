using Metricaly.Core.Common;
using Metricaly.Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Services
{
    public class RedisMetricsClient : IMetricsClient
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;
        private LoadedLuaScript loadedLuaScript;

        public RedisMetricsClient(IConnectionMultiplexer connectionMultiplexer)
        {
            this.connectionMultiplexer = connectionMultiplexer;
            Init();
        }

        private void Init()
        {
            const string script =
                       "local count = tonumber(@count) " +
                       "local max = tonumber(@max) " +
                       "local min = tonumber(@min) " +
                       "local sum = tonumber(@sum) " +
                       "local timestamp = tonumber(@timestamp) " +
                       "local oldValues = redis.call('zrangebyscore', @metricKey, timestamp, timestamp) " +
                       "if (next(oldValues) == nil) " +
                       "then " +
                       "  redis.call('zadd', @metricKey, timestamp, timestamp .. ' ' .. count .. ' ' .. max .. ' ' .. min .. ' ' .. sum)" +
                       "else " +
                       "  local s = oldValues[1] " +
                       "  local oldData = {} " +
                       "  for word in s:gmatch('%S+') do table.insert(oldData, word) end " +
                       "  local oldCount = tonumber(oldData[2]) " +
                       "  local oldMax = tonumber(oldData[3]) " +
                       "  local oldMin = tonumber(oldData[4]) " +
                       "  local oldSum = tonumber(oldData[5]) " +
                       "  redis.call('zremrangebyscore', @metricKey, timestamp, timestamp) " +
                       "  redis.call('zadd', @metricKey, timestamp, timestamp .. ' ' .. count + oldCount .. ' ' .. math.max(oldMax, max) .. ' ' .. math.min(oldMin, min) .. ' ' .. sum + oldSum) " +
                       "end";

            var server = connectionMultiplexer.GetServer("redis:6379");

            var prepared = LuaScript.Prepare(script);
            loadedLuaScript = prepared.Load(server);
        }

        public bool CollectMetric(string metricKey, MetricValue value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CollectMetricAsync(string metricKey, MetricValue value)
        {
            throw new NotImplementedException();
        }

        public  MetricValue[] QueryMetrics(string metricKey, long startTimestamp, long endTimestamp)
        {
            throw new NotImplementedException();
        }

        public async Task<MetricValue[]> QueryMetricsAsync(string metricKey, long startTimestamp, long endTimestamp)
        {
            var redisDb = connectionMultiplexer.GetDatabase();

            var result = await redisDb.SortedSetRangeByScoreWithScoresAsync(metricKey, startTimestamp, endTimestamp);

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
