using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Services;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metricaly.Angular;
using Metricaly.Infrastructure.Common.Extensions;

namespace Playground
{
    public class Solution_2
    {
        private ConnectionMultiplexer conn;
        private const int DBNbr = 1;
        private static Random rand = new Random();

        public Solution_2(ConnectionMultiplexer conn)
        {
            this.conn = conn;
        }

        public async Task Run()
        {
            var redisDb = conn.GetDatabase(DBNbr);
            //await TestResults();

            //await TestSortedSet();

            await ConsumersManager();

            //var consumerTask = RunConsumer("consumer_1");
            //var consumerTask2 = RunConsumer("consumer_2");


            var stopwatch = Stopwatch.StartNew();
            for (int i = 1; i <= 1_000_000; i++)
            {
                try
                {
                    var result = new AggregatedMetric()
                    {
                        Count = i,
                        Max = i,
                        Min = i,
                        Sum = i,
                        Timestamp = 1602414000 + i
                    };

                    await redisDb.SortedSetAddAsync("test2", JsonConvert.SerializeObject(result), result.Timestamp);

                    if (i % 10000 == 0)
                    {
                        stopwatch.Stop();
                        Console.WriteLine($"Took: {i}  -  {stopwatch.ElapsedMilliseconds} ms.");
                        stopwatch = Stopwatch.StartNew();
                    }

                    //await HandleRequest("Metric_" + 1, i);
                    //await HandleRequest("Metric_" + 2, i);
                    //await HandleRequest("Metric_" + 3, i);
                    //await HandleRequest("Metric_" + 4, i);
                    //await HandleRequest("Metric_" + 5, i);
                    //await HandleRequest("Metric_" + 6, i);
                }
                catch (Exception ex)
                { }
            }

            //Task.WaitAll(consumerTask, consumerTask2);
        }

        private async Task TestResults()
        {
            var sortedSetName = "metric.values:Metric_1";
            var redisDb = conn.GetDatabase(DBNbr);

            var entries = await redisDb.SortedSetRangeByScoreAsync<AggregatedMetric>(sortedSetName);

            long sumTotal = 0;
            long countTotal = 0;

            foreach (var item in entries)
            {
                sumTotal += item.Sum;
                countTotal += item.Count;
            }

        }

        private async Task HandleRequest(string metricName, long value)
        {
            var redisDb = conn.GetDatabase(DBNbr);

            var currentTimeStampInMicroseconds = TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Microseconds);

            // Decide in which consumer we should put this metric
            var metricConsumerName = await GetMetricConsumer(conn, metricName, currentTimeStampInMicroseconds);

            // Put the metric value into the consumer's stream
            var nameValueEntry = new NameValueEntry[] {
                new NameValueEntry("metricName", metricName),
                new NameValueEntry("value", value),
                new NameValueEntry("time", currentTimeStampInMicroseconds),
            };
            await redisDb.StreamAddAsync("stream:" + metricConsumerName, nameValueEntry);
        }

        private async Task ConsumersManager()
        {
            var redisDb = conn.GetDatabase();

            var consumers = new List<string>() { "consumer_1",/* "consumer_2", "consumer_3", "consumer_4", "consumer_5"*/ };

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

            //while (true)
            //{
            //    // Monitor the consumers and if any of them broke remove it from the round robin list
            //    await Task.Delay(1000);
            //}
        }

        private async Task<string> GetMetricConsumer(ConnectionMultiplexer conn, string metricName, long currentTimeStampInMicroseconds)
        {
            // consumer-metric key
            // consumers.list key
            // return consumer-name

            var currentTimeStampInSeconds = (long)(Math.Floor((currentTimeStampInMicroseconds / 1_000_000) / 60.0) * 60);
            var metricId = metricName + ":" + currentTimeStampInSeconds;

            var redisDb = conn.GetDatabase(DBNbr);

            string Script = "local value = redis.call('get', @key) " +
                "if (value == false) then " +
                " local consumer = redis.call('rpoplpush', @list, @list) " +
                " if (consumer == false) then return nil end " +
                " redis.call('set', @key, consumer, 'EX', 172000) " +
                " return consumer " +
                "else " +
                " return value " +
                "end ";

            var prepared = LuaScript.Prepare(Script);
            var consumer = await redisDb.ScriptEvaluateAsync(prepared, new { key = (RedisKey)("assigned.consumer:" + metricId), list = (RedisKey)"consumers.list" });

            var value = consumer.ToString();
            return value;
        }

        private async Task RunConsumer(string consumerName)
        {
            var redisDb = conn.GetDatabase(DBNbr);
            var consumerStreamName = "stream:" + consumerName;
            var chunckReadCount = 500;
            var totalRaw = 0;
            try
            {
                //await redisDb.StreamDeleteConsumerGroupAsync(consumerStreamName, "metrics_consumer_group");
                await redisDb.StreamCreateConsumerGroupAsync(consumerStreamName, "metrics_consumer_group", "0");
            }
            catch (Exception ex)
            { }

            while (true)
            {
                var entries = await redisDb.StreamReadGroupAsync(consumerStreamName, "metrics_consumer_group", consumerName, ">", count: chunckReadCount);

                if (entries.Length == 0)
                {
                    await Task.Delay(250);
                    continue;
                }

                Stopwatch stopwatch = Stopwatch.StartNew();
                totalRaw += entries.Length;

                // Parse the metrics from the Stream's entries
                var metrics = new List<Metric>();
                foreach (var entry in entries)
                {
                    var metricName = entry.Values.FirstOrDefault(x => x.Name == "metricName").Value.ToString();
                    var value = long.Parse(entry.Values.FirstOrDefault(x => x.Name == "value").Value);
                    var time = long.Parse(entry.Values.FirstOrDefault(x => x.Name == "time").Value);

                    metrics.Add(new Metric()
                    {
                        StreamMessageId = entry.Id,
                        Name = metricName,
                        Timestamp = time,
                        Value = value,
                        TimestampGranulated = (long)Math.Floor(time / 1_000_000M)
                    });
                }

                // Group the metrics by name, and then by timestamp, then aggregate them
                var aggregatedMetrics = new List<AggregatedMetricCollection>();
                var groupedMetricsByName = metrics.GroupBy(x => x.Name);
                foreach (var metricGroup in groupedMetricsByName)
                {
                    var aggregatedMetricCollection = new AggregatedMetricCollection();
                    aggregatedMetricCollection.MetricName = metricGroup.Key;

                    var metricsByTime = metricGroup.ToList().GroupBy(x => x.TimestampGranulated);
                    foreach (var metric in metricsByTime)
                    {
                        var aggregatedMetric = AggregateMetricByTime(metricGroup.ToList());
                        aggregatedMetricCollection.MetricsByTime = aggregatedMetric;
                    }
                    aggregatedMetrics.Add(aggregatedMetricCollection);
                }


                // Params needed for each aggregated metric are:
                //      metricName (the sorted set that have the values of this metric)
                //      metric json value (timestamp, max, min, count, sum)
                //      metric timestamp (the score in the sorted set)
                foreach (var metricCollection in aggregatedMetrics)
                {
                    foreach (var metricByTime in metricCollection.MetricsByTime)
                    {
                        var oldMetricValues = await redisDb.SortedSetRangeByScoreAsync<AggregatedMetric>(metricCollection.SortedSetName, metricByTime.Timestamp, metricByTime.Timestamp);
                        if (oldMetricValues.Length == 0)
                        {
                            // Insert this metric value
                            await redisDb.SortedSetAddAsync(metricCollection.SortedSetName, JsonConvert.SerializeObject(metricByTime), metricByTime.Timestamp);
                        }
                        else
                        {
                            // Aggregate these two aggregations together and insert the result
                            var oldMetricValue = oldMetricValues[0];

                            var result = new AggregatedMetric()
                            {
                                Count = metricByTime.Count + oldMetricValue.Count,
                                Max = Math.Max(metricByTime.Max, oldMetricValue.Max),
                                Min = Math.Min(metricByTime.Min, oldMetricValue.Min),
                                Sum = metricByTime.Sum + oldMetricValue.Sum,
                                Timestamp = oldMetricValue.Timestamp
                            };

                            var removedCount = await redisDb.SortedSetRemoveRangeByScoreAsync(metricCollection.SortedSetName, oldMetricValue.Timestamp, oldMetricValue.Timestamp);

                            if (removedCount != 1)
                            { }

                            await redisDb.SortedSetAddAsync(metricCollection.SortedSetName, JsonConvert.SerializeObject(result), result.Timestamp);

                            Console.WriteLine($"Old metric aggregated with new one at time {oldMetricValue.Timestamp}.");
                        }
                    }
                }
                stopwatch.Stop();

                Console.WriteLine($"Aggregation results from {metrics.Count} raw data points.");
                foreach (var item in aggregatedMetrics)
                {
                    Console.WriteLine($" * {item.MetricName}  :");
                    foreach (var metric in item.MetricsByTime)
                    {
                        Console.WriteLine($"\t\t {metric.Timestamp}  =>  sum:{metric.Sum}, count:{metric.Count}, min:{metric.Min}, max:{metric.Max}");
                    }
                }

                // Acknowledge the read metrics
                var messagesIds = new RedisValue[entries.Length];
                for (int i = 0; i < entries.Length; i++)
                {
                    messagesIds[i] = entries[i].Id;
                }
                var ackNumberOfMessages = await redisDb.StreamAcknowledgeAsync(consumerStreamName, "metrics_consumer_group", messagesIds);

                if (ackNumberOfMessages != entries.Length)
                { }

                Console.WriteLine($"Consumer {consumerName} acknowledged {ackNumberOfMessages} messages.");
                Console.WriteLine($"--------------------------- Took: {stopwatch.ElapsedMilliseconds} ms --------------------------");

                if (entries.Length <= chunckReadCount)
                {
                    await Task.Delay(500);
                }
            }
        }


        private List<AggregatedMetric> AggregateMetricByTime(List<Metric> metrics)
        {
            var metricsByTime = metrics.GroupBy(x => x.TimestampGranulated);

            var collection = new List<AggregatedMetric>();

            foreach (var metricGroup in metricsByTime)
            {
                var temp = metricGroup.ToList();

                var aggregatedMetric = new AggregatedMetric()
                {
                    Count = temp.Count,
                    Sum = temp.Sum(x => x.Value),
                    Max = temp.Max(x => x.Value),
                    Min = temp.Min(x => x.Value),
                    Timestamp = metricGroup.Key
                };

                collection.Add(aggregatedMetric);
            }

            return collection;
        }

        public async Task TestSortedSet()
        {
            var redisDb = conn.GetDatabase();

            Stopwatch stopwatch = Stopwatch.StartNew();

            var random = "aaaaaaaaaaaaaa";
            for (int i = 1; i <= 1000; i++)
            {
                await redisDb.SortedSetAddAsync("small", random + i, i);
                await redisDb.SortedSetAddAsync("big", "value_" + i, i);
            }

            // var result = await redisDb.SortedSetRangeByScoreWithScoresAsync("sortedset_benchmark");

            stopwatch.Stop();

            Console.WriteLine($" - Adding to sorted set took: {stopwatch.ElapsedMilliseconds} ms.");
        }

        private class Metric
        {
            public string Name { get; set; }
            public long Value { get; set; }
            public long Timestamp { get; set; }
            public long TimestampGranulated { get; set; }
            public RedisValue StreamMessageId { get; internal set; }
        }

        private class AggregatedMetric
        {
            //public string Name { get; set; }
            public long Timestamp { get; set; }
            public long Max { get; set; }
            public long Min { get; set; }
            public long Sum { get; set; }
            public long Count { get; set; }
        }

        private class AggregatedMetricCollection
        {
            public string MetricName { get; set; }
            public string SortedSetName => "metric.values:" + MetricName;
            public List<AggregatedMetric> MetricsByTime { get; set; } = new List<AggregatedMetric>();
        }
    }
}
