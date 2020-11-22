using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Playground
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [RPlotExporter]
    public class LuaBenchmark : IDisposable
    {
        private static Random rand = new Random();
        private ConnectionMultiplexer conn;
        private LoadedLuaScript loaded;
        private IDatabase db;

        private List<string> metrics = new List<string> { "M1", "M2", "M2", "M3", "M4", "M5", "M6", "M7", "M8", "M10" };

        [GlobalSetup]
        public void Setup()
        {
            string script =
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

            conn = ConnectionMultiplexer.Connect("127.0.0.1:7001");

            db = conn.GetDatabase(2);
            var server = conn.GetServer("127.0.0.1", 7001);

            var prepared = LuaScript.Prepare(script);
            loaded = prepared.Load(server);
        }


        [Benchmark]
        public void Run()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var metric = metrics[rand.Next(metrics.Count)];

            loaded.Evaluate(db, new
            {
                metricKey = (RedisKey)metric,
                timestamp,
                count = 1,
                min = 1,
                max = 1,
                sum = 1,
            });
        }

        public void Dispose()
        {
            conn.Dispose();
        }
    }
}
