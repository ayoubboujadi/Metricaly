using AdysTech.InfluxDB.Client.Net;
using Metricaly.Angular.MetricServices;
using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Interfaces;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Metricaly.Angular;

namespace Metricaly.Web
{
    public class AggregatedMetricData
    {
        public string MetricId { get; set; }
        public long Timestamp { get; set; }


        [InfluxDBMeasurementName]
        public string Measurement { get; set; }

        [InfluxDBTime]
        public DateTime Time { get; set; }

        [InfluxDBPrecision]
        public TimePrecision Precision { get; set; }


        [InfluxDBField("count")]
        public long Count { get; set; }

        [InfluxDBField("sum")]
        public long Sum { get; set; }

        [InfluxDBField("min")]
        public long Min { get; set; }

        [InfluxDBField("max")]
        public long Max { get; set; }

        [InfluxDBField("average")]
        public double Average { get; set; }
    }


    public class MetricsAggregator
    {
        private const double GRANULARITY = 10.0;

        private class GroupedMetricData
        {
            public string MetricId { get; set; }
            public long Timestamp { get; set; }

            public List<MetricData> Metrics { get; set; }
        }

        private class MetricData
        {
            public string MetricId { get; set; }
            public long TimespanInMicroseconds { get; set; }
            public long DownsampledTimestampInSeconds { get; set; }
            public long Value { get; set; }

            public static List<MetricData> Parse(SortedSetEntry[] sortedSetEntries)
            {
                var metrics = new List<MetricData>();
                foreach (var sortedSetEntry in sortedSetEntries)
                {
                    string key = sortedSetEntry.Element.ToString().Trim('"');
                    var data = key.Split(new string[] { ":" }, StringSplitOptions.None);

                    var metricData = new MetricData
                    {
                        MetricId = data[0],
                        Value = long.Parse(data[1]),
                        TimespanInMicroseconds = (long)sortedSetEntry.Score,
                    };

                    var timespanInSeconds = metricData.TimespanInMicroseconds / 1_000_000;
                    metricData.DownsampledTimestampInSeconds = (long)(Math.Floor(timespanInSeconds / GRANULARITY) * GRANULARITY);

                    metrics.Add(metricData);
                }

                return metrics;
            }

            public static IEnumerable<GroupedMetricData> GroupByMetricId(List<MetricData> metricsData)
            {
                var result = metricsData
                    .GroupBy(x => new { x.MetricId, x.DownsampledTimestampInSeconds })
                    .Select(x => new GroupedMetricData
                    {
                        MetricId = x.Key.MetricId,
                        Timestamp = x.Key.DownsampledTimestampInSeconds,
                        Metrics = x.ToList()
                    });

                return result;
            }
        }



        private readonly IConnectionMultiplexer connectionMultiplexer;
        private readonly InfluxDBClient influxDBClient;

        public MetricsAggregator(IConnectionMultiplexer connectionMultiplexer, InfluxDBClient influxDBClient)
        {
            this.connectionMultiplexer = connectionMultiplexer;
            this.influxDBClient = influxDBClient;
        }

        public async Task<int> AggregateAsync(string metricKey)
        {
            var redisDb = connectionMultiplexer.GetDatabase();

            // Read newly added values
            var metricValues = await redisDb.SortedSetRangeByRankWithScoresAsync<MetricRedisValue>(metricKey, 0, -1);

            // Create the metric if it doesn't exist
            var metricId = "0";

            var timestamp = metricValues.FirstOrDefault().TimestampGranulated;

            var aggregatedMetricData = new AggregatedMetricData
            {
                Measurement = "m_" + metricId,
                MetricId = metricId,
                Sum = metricValues.Sum(x => x.Value),
                Average = metricValues.Average(x => x.Value),
                Min = metricValues.Min(x => x.Value),
                Max = metricValues.Max(x => x.Value),
                Count = metricValues.Length,
                Timestamp = timestamp,
                Precision = TimePrecision.Seconds,
            };

            aggregatedMetricData.Time = DateTimeOffset.FromUnixTimeSeconds(aggregatedMetricData.Timestamp).UtcDateTime;

            // Insert this metric to InfluxDb
            var datapoint = new InfluxDatapoint<InfluxValueField>
            {
                MeasurementName = "m_" + aggregatedMetricData.MetricId,
                UtcTimestamp = DateTimeOffset.FromUnixTimeSeconds(aggregatedMetricData.Timestamp).UtcDateTime,
                Precision = TimePrecision.Seconds
            };

            datapoint.Fields.Add("sum", new InfluxValueField(aggregatedMetricData.Sum));
            datapoint.Fields.Add("average", new InfluxValueField(aggregatedMetricData.Average));
            datapoint.Fields.Add("min", new InfluxValueField(aggregatedMetricData.Min));
            datapoint.Fields.Add("max", new InfluxValueField(aggregatedMetricData.Max));
            datapoint.Fields.Add("count", new InfluxValueField(aggregatedMetricData.Count));

            var result = await influxDBClient.PostPointAsync("myDatabase", datapoint);

            // Remove the metric's sorted set
            await redisDb.KeyDeleteAsync(metricKey);

            return metricValues.Length;
        }
    }
}
