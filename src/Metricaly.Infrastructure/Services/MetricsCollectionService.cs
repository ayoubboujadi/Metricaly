using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Common.Extensions;
using Metricaly.Infrastructure.Interfaces;
using Metricaly.Infrastructure.Services;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Metricaly.Web
{
    public class MetricsCollectionService : IMetricsCollectionService
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;
        private LoadedLuaScript loadedLuaScript;

        public MetricsCollectionService(IConnectionMultiplexer connectionMultiplexer)
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

            // TODO: Get the host.proxy from the config
            var server = connectionMultiplexer.GetServer("127.0.0.1", 7001);

            var prepared = LuaScript.Prepare(script);
            loadedLuaScript = prepared.Load(server);
        }

        public async Task CollectAsync(Guid applicationId, string metricName, string metricNamespace, double value)
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

        public async Task StoreOldTwo(IMetricRepository metricRepository, Guid applicationId, string metricName, string metricNamespace, long value)
        {
            var redisDb = connectionMultiplexer.GetDatabase();
            var currentTimeStampInMicroseconds = TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Microseconds);
            var metricKey = $"{applicationId}:{metricNamespace}:{metricName}";

            // Decide in which consumer we should put this metric
            var metricConsumerName = await GetMetricConsumer(metricKey, currentTimeStampInMicroseconds);

            var dbMetric = await metricRepository.FindAsync(applicationId, metricNamespace, metricName);
            if(dbMetric == null)
            {
                await metricRepository.InsertAsync(dbMetric);
            }

            var nameValueEntry = new NameValueEntry[] {
                new NameValueEntry("appId", applicationId.ToString()),
                new NameValueEntry("name", metricName),
                new NameValueEntry("namespace", metricNamespace),
                new NameValueEntry("value", value),
                new NameValueEntry("time", currentTimeStampInMicroseconds),
            };

            // Put the metric value into the consumer's stream
            await redisDb.StreamAddAsync("stream:" + metricConsumerName, nameValueEntry);
        }

        private async Task<string> GetMetricConsumer(string metricKey, long currentTimeStampInMicroseconds)
        {
            // consumer-metric key
            // consumers.list key
            // return consumer-name
            var redisDb = connectionMultiplexer.GetDatabase();

            var currentTimeStampInSeconds = (long)(Math.Floor((currentTimeStampInMicroseconds / 1_000_000) / 60.0) * 60);
            RedisKey assignedConsumerKey = $"sortedset:assigned.consumer:{metricKey}";

            string script =
                //"local value = redis.call('get', @key) " +
                "local values = redis.call('zrange', @key, @time, @time) " +
                "if (next(values) == nil) then " +
                " local consumer = redis.call('rpoplpush', @list, @list) " +
                " if (consumer == false) then return nil end " +
                //" redis.call('set', @key, consumer, 'EX', 172000) " +
                " redis.call('zadd', @key, @time, consumer .. '+' ..  @time) " + // Add the consumer to the metric's set
                " redis.call('zadd', consumer .. ':assigned.metrics', 0, @key .. ':' .. @time) " + // Add the metric to the consumer's set
                " return consumer " +
                "else " +
                " return values[0] " +
                "end ";

            var prepared = LuaScript.Prepare(script);
            var consumer = await redisDb.ScriptEvaluateAsync(prepared, new { key = assignedConsumerKey, list = (RedisKey)"consumers.list", time = currentTimeStampInSeconds });

            if (consumer.IsNull)
            {
                return "default";
            }

            return consumer.ToString();
        }

        //public async Task StoreOld(long applicationId, string metricName, string metricNamespace, long value)
        //{
        //    var redisDb = connectionMultiplexer.GetDatabase();

        //    var currentTimeStampInMicroseconds = TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Microseconds);
        //    var currentTimeStampInSeconds = (long)(Math.Floor((currentTimeStampInMicroseconds / 1_000_000) / 10.0) * 10);

        //    var metricRedisValue = new MetricRedisValue()
        //    {
        //        ApplicationId = applicationId,
        //        Namespace = metricNamespace,
        //        MetricName = metricName,
        //        Timestamp = currentTimeStampInMicroseconds,
        //        TimestampGranulated = currentTimeStampInSeconds,
        //        Value = value
        //    };

        //    var redisMetricKey = "m:" + applicationId + "." + metricNamespace + "." + metricName + ":" + currentTimeStampInSeconds;

        //    var added = await redisDb.SortedSetAddAsync(redisMetricKey, metricRedisValue, currentTimeStampInMicroseconds);

        //    var added2 = await redisDb.SortedSetAddAsync("recentlyset", redisMetricKey, currentTimeStampInSeconds + 10);
        //}
    }
}
