using AdysTech.InfluxDB.Client.Net;
using BenchmarkDotNet.Running;
using Metricaly.Core.Common.Utils;
using Metricaly.Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Playground
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<LuaBenchmark>();
            MetricsCollector collector = new MetricsCollector();
            await collector.Start();

            return;
            //await BenchmarkLua();


            using (var conn = ConnectionMultiplexer.Connect("127.0.0.1:7001"))
            {
                //await ConsumersManager(conn);

                await RunConsumer(conn, "consumer_1");

                //await RedisRequestHandler(conn, "Metric 1", 1000);
                //await RedisRequestHandler(conn, "Metric 1", 2000);
                //await RedisRequestHandler(conn, "Metric 1", 3000);
                //await RedisRequestHandler(conn, "Metric 1", 4000);
                //await RedisRequestHandler(conn, "Metric 2", 20);
                //await RedisRequestHandler(conn, "Metric 2", 21);
                //await RedisRequestHandler(conn, "Metric 2", 22);
            }

            //await TestBenchmark();
        }

        private static async Task ConsumersManager(ConnectionMultiplexer conn)
        {
            var redisDb = conn.GetDatabase();

            var consumers = new List<string>() { "consumer_1", "consumer_2", "consumer_3" /*, "consumer_4", "consumer_5"*/ };

            // Prepare the consumers sorted set, it'll have the list of all consumers with the last time they were actually running
            foreach (var consumer in consumers)
            {
                // Add this consumer to the available consumers sorted set
                await redisDb.SortedSetAddAsync("consumers.sortedset", consumer, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            }

            // Prepare the consumers round robin list
            foreach (var consumer in consumers)
            {
                await redisDb.ListLeftPushAsync("consumers.list", consumer);
            }

            while (true)
            {
                // Monitor the consumers and if any of them broke remove it from the round robin list
                await Task.Delay(1000);
            }
        }

        private static async Task RunConsumer(ConnectionMultiplexer conn, string consumerName)
        {
            var redisDb = conn.GetDatabase();

            while (true)
            {
                var currentSeconds = TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Seconds, 10);

                var sortedSetEntries = await redisDb.SortedSetRangeByScoreWithScoresAsync("recentlyset:" + consumerName, double.NegativeInfinity, currentSeconds, take: 1);

                if (sortedSetEntries.Length == 1)
                {
                    var metricId = sortedSetEntries[0].Element.ToString();
                    var metricKey = metricId.Split(':')[1];
                    var timestamp = long.Parse(metricId.Split(':')[2]);

                    // Aggreagate this metric
                    var metricValues = await redisDb.SortedSetRangeByRankWithScoresAsync(metricId, 0, -1);
                    var values = new List<long>();
                    foreach (var metricValue in metricValues)
                    {
                        var temp = metricValue.Element.ToString();
                        values.Add(long.Parse(temp));
                    }

                    var aggregatedMetric = new AggregatedMetric()
                    {
                        Count = values.Count,
                        Max = values.Max(),
                        Min = values.Min(),
                        Sum = values.Sum(),
                        Timestamp = timestamp
                    };

                    var isInserted = await redisDb.SortedSetAddAsync(metricKey + ":values", aggregatedMetric.ToString(), timestamp);

                    // delete the recentry set metric
                    var deletedCount = await redisDb.SortedSetRemoveRangeByScoreAsync("recentlyset:" + consumerName, sortedSetEntries[0].Score, sortedSetEntries[0].Score);

                    // delete the metric's values in the specific time range
                    var metricValuesDeleted = await redisDb.KeyDeleteAsync(metricId);

                    // Remove the metric assigned to consumer key
                    var assignedKeyRemoved = await redisDb.KeyDeleteAsync("assigned.consumer:" + metricId);
                }
                else
                {
                    await Task.Delay(100);
                }
            }

        }

        private static async Task RedisRequestHandler(ConnectionMultiplexer conn, string metricName, long value)
        {
            var redisDb = conn.GetDatabase();

            var currentTimeStampInMicroseconds = TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Microseconds);
            var currentTimeStampInSeconds = (long)(Math.Floor((currentTimeStampInMicroseconds / 1_000_000) / 10.0) * 10);

            var redisMetricKey = "m:" + metricName + ":" + currentTimeStampInSeconds;

            // Insert the value of this metric
            var added = await redisDb.SortedSetAddAsync(redisMetricKey, value, currentTimeStampInMicroseconds);

            // Check if the metric at this timestamp was already assigned into some consumer
            var consumer = await GetMetricConsumer(conn, redisMetricKey);


            // Add this metric to the consumer's 
            var added2 = await redisDb.SortedSetAddAsync("recentlyset:" + consumer, redisMetricKey, currentTimeStampInSeconds + 10);
        }

        private static async Task<string> GetMetricConsumer(ConnectionMultiplexer conn, string redisMetricKey)
        {
            // consumer-metric key
            // consumers.list key
            // return consumer-name

            var redisDb = conn.GetDatabase();

            string Script = "local value = redis.call('get', @key) " + // Check if this metric got assigned to a consumer before
                "if (value == false) then " +
                " local consumer = redis.call('rpoplpush', @list, @list) " + // Get a new consumer
                " if (consumer == false) then return nil end " +
                " redis.call('set', @key, consumer) " + // Assign the consumer to the metric
                "" + // TODO: Add the metric in the consumers set
                " return consumer " +
                "else " +
                " return value " +
                "end ";

            var prepared = LuaScript.Prepare(Script);
            var consumer = await redisDb.ScriptEvaluateAsync(prepared, new { key = (RedisKey)("assigned.consumer:" + redisMetricKey), list = (RedisKey)"consumers.list" });

            //// Check if the metric at this timestamp was already assigned into some consumer
            //var consumer = await redisDb.StringGetAsync("consumer:" + redisMetricKey);

            //if (consumer.IsNull)
            //{
            //    // This metric was never assigned to a consumer before
            //    // Find a suitable consumer for this metric
            //    consumer = await redisDb.ListRightPopLeftPushAsync("consumers.list", "consumers.list");

            //    // Set this metric's consumer
            //    var metricConsumerAdded = await redisDb.StringSetAsync("consumer: " + redisMetricKey, consumer);
            //}


            var value = consumer.ToString();
            return value;
        }


        private static HttpClient httpClient = new HttpClient();
        static async Task TestBenchmark()
        {
            httpClient.DefaultRequestHeaders.Add("ApiKey", "KR6EdgVl46yvV3fDIEjrgdBgzwcYxpLBZHTxQPLfy2g=");

            var metrics = new List<string>();
            for (int i = 1; i <= 1000; i++)
            {
                metrics.Add("Metric " + i);
            }
            int maxThreads = 50;
            int threadsCount = 0;
            var stopwatchAll = Stopwatch.StartNew();
            for (int i = 0; i < 2; i++)
            {
                foreach (var metric in metrics)
                {
                    while (threadsCount >= maxThreads)
                        Thread.Sleep(25);

                    Interlocked.Increment(ref threadsCount);

                    var task = Task.Run(async () =>
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        var result = await httpClient.GetAsync($"https://localhost:5001/apimetric/add/default/{metric}/100");
                        stopwatch.Stop();
                        Console.WriteLine(i + " - " + threadsCount + " \t " + result.StatusCode + " \t " + stopwatch.ElapsedMilliseconds + " ms");
                    }).ContinueWith(task => Interlocked.Decrement(ref threadsCount));
                }
            }
            stopwatchAll.Stop();
            Console.WriteLine("All took : " + stopwatchAll.ElapsedMilliseconds);
        }


        private static async Task BenchmarkLua()
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


            using (ConnectionMultiplexer conn = ConnectionMultiplexer.Connect("127.0.0.1:7001"))
            {
                var db = conn.GetDatabase(0);
                var server = conn.GetServer("127.0.0.1", 7001);

                var prepared = LuaScript.Prepare(script);
                var loaded = prepared.Load(server);
                Console.WriteLine("Hash: " + HexStringFromBytes(loaded.Hash));

                var stopwatch = Stopwatch.StartNew();
                int timestamp = 11000;
                for (int i = 1; i <= 100; i++)
                {
                    if (i % 100 == 0)
                        timestamp++;

                    await loaded.EvaluateAsync(db, new
                    {
                        metricKey = (RedisKey)"Metric 2",
                        timestamp,
                        count = 1,
                        min = 1,
                        max = 1,
                        sum = 1,
                    });
                }

                stopwatch.Stop();

                Console.WriteLine($"Lua script took : " + stopwatch.ElapsedMilliseconds);
            }

        }
        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        private class AggregatedMetric
        {
            public long Timestamp { get; set; }
            public long Max { get; set; }
            public long Min { get; set; }
            public long Sum { get; set; }
            public long Count { get; set; }


            public override string ToString()
            {
                return $"{Timestamp} {Count} {Sum} {Min} {Max}";
            }
        }

    }

}



// CREATE CONTINUOUS QUERY "cq_basic_br" ON "myDatabase" BEGIN SELECT mean(*), sum(*), min(*), max(*), count(*) INTO "myDatabaseDownsampled"."autogen".:MEASUREMENT FROM /.*/ GROUP BY time(10s),* END


