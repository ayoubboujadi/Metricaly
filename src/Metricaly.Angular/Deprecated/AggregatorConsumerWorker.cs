using Metricaly.Infrastructure.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Angular.MetricServices
{
    public class AggregatorConsumerWorker : BackgroundService
    {
        private readonly string consumerName = "consumer_1";

        private readonly IConnectionMultiplexer connectionMultiplexer;
        private readonly ILogger<AggregatorConsumerWorker> logger;

        public AggregatorConsumerWorker(IConnectionMultiplexer connectionMultiplexer, ILogger<AggregatorConsumerWorker> logger)
        {
            this.connectionMultiplexer = connectionMultiplexer;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var redisDb = connectionMultiplexer.GetDatabase();
            var consumerStreamName = "stream:" + consumerName;

            try
            {
                await redisDb.StreamCreateConsumerGroupAsync(consumerStreamName, "metrics_consumer_group", "0");
            }
            catch (Exception ex)
            { }

            while (!stoppingToken.IsCancellationRequested)
            {
                var chunckReadCount = 500;

                while (true)
                {
                    var entries = await redisDb.StreamReadGroupAsync(consumerStreamName, "metrics_consumer_group", consumerName, ">", count: chunckReadCount);

                    if (entries.Length == 0)
                    {
                        await Task.Delay(250);
                        continue;
                    }

                    // Parse the metrics from the Stream's entries
                    var metrics = new List<Metric>();
                    foreach (var entry in entries)
                    {
                        var time = long.Parse(entry.Values.FirstOrDefault(x => x.Name == "time").Value);
                        metrics.Add(new Metric()
                        {
                            StreamMessageId = entry.Id,
                            Name = entry.Values.FirstOrDefault(x => x.Name == "name").Value.ToString(),
                            ApplicationId = entry.Values.FirstOrDefault(x => x.Name == "appId").Value.ToString(),
                            Namespace = entry.Values.FirstOrDefault(x => x.Name == "namespace").Value.ToString(),
                            Value = long.Parse(entry.Values.FirstOrDefault(x => x.Name == "value").Value),
                            Timestamp = time,
                            TimestampGranulated = (long)Math.Floor(time / 1_000_000M)
                        });
                    }

                    // Group the metrics by name, and then by timestamp, then aggregate them
                    var aggregatedMetrics = new List<AggregatedMetricCollection>();
                    var groupedMetricsByName = metrics.GroupBy(x => x.UniqueKey);
                    foreach (var metricGroup in groupedMetricsByName)
                    {
                        var aggregatedMetricCollection = new AggregatedMetricCollection();
                        aggregatedMetricCollection.UniqueKey = metricGroup.Key;

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
                            }
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

                    logger.LogInformation($"Aggregator Worker {consumerName} finished with {ackNumberOfMessages}.");

                    if (entries.Length <= chunckReadCount)
                    {
                        await Task.Delay(500);
                    }
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


        private class Metric
        {
            public string Name { get; set; }
            public string Namespace { get; set; }
            public string ApplicationId { get; set; }
            public string UniqueKey => $"{ApplicationId}:{Namespace}:{Name}";
            public long Value { get; set; }
            public long Timestamp { get; set; }
            public long TimestampGranulated { get; set; }
            public RedisValue StreamMessageId { get; internal set; }
        }

        private class AggregatedMetric
        {
            public long Timestamp { get; set; }
            public long Max { get; set; }
            public long Min { get; set; }
            public long Sum { get; set; }
            public long Count { get; set; }
        }

        private class AggregatedMetricCollection
        {
            public string MetricName { get; set; }
            public string Namespace { get; set; }
            public string ApplicationId { get; set; }
            public string UniqueKey { get; set; }
            
            public string SortedSetName => $"metric.values:{UniqueKey}";
            public List<AggregatedMetric> MetricsByTime { get; set; } = new List<AggregatedMetric>();
        }

    }
}
