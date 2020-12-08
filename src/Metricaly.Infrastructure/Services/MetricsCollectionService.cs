﻿using Metricaly.Core.Common.Utils;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Common.Models;
using Metricaly.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Metricaly.Web
{
    public class MetricsCollectionService : IMetricsCollectionService
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;
        private readonly RedisSettings redisSettings;

        private LoadedLuaScript loadedLuaScript;

        public MetricsCollectionService(IConnectionMultiplexer connectionMultiplexer, IOptions<RedisSettings> options)
        {
            this.connectionMultiplexer = connectionMultiplexer;
            this.redisSettings = options.Value;

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

            var server = connectionMultiplexer.GetServer(redisSettings.Host);

            var prepared = LuaScript.Prepare(script);
            loadedLuaScript = prepared.Load(server);
        }

        public async Task CollectSingleAggregateAsync(Guid applicationId, string metricName, string metricNamespace, double value)
        {
            var timestamp = TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Seconds);
            var redisDb = connectionMultiplexer.GetDatabase();
            var metricKey = $"{applicationId}:{metricNamespace}:{metricName}";

            await loadedLuaScript.EvaluateAsync(redisDb, new
            {
                metricKey = (RedisKey)metricKey,
                timestamp,
                count = 1,
                min = value,
                max = value,
                sum = value,
            });
        }
    }
}
