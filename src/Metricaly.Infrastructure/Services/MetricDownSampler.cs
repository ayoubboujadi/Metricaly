using Metricaly.Core.Common;
using Metricaly.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metricaly.Infrastructure.Services
{
    public class MetricDownSampler : IMetricDownSampler
    {
        public MetricDownsaplingResult DownSample(MetricValue[] metricValues, int preferedSamplingValue, long startTimestamp, long endTimestamp)
        {
            var sampledMetricValues = new List<MetricValue>();

            int samplingValue = GetSampleValue(startTimestamp, endTimestamp); // In seconds

            if (samplingValue < preferedSamplingValue)
            {
                samplingValue = preferedSamplingValue;
            }

            // Calculate start timestamp based on the sampling value
            startTimestamp = (long)Math.Floor(startTimestamp / (decimal)samplingValue) * samplingValue;

            for (long timestamp = startTimestamp; timestamp <= endTimestamp; timestamp += samplingValue)
            {
                var metricsInTimeframe = metricValues.Where(x => x.TimeStamp >= timestamp && x.TimeStamp < timestamp + samplingValue);
                if (metricsInTimeframe.Any())
                {
                    var aggregatedValue = AggregateMetricsValues(metricsInTimeframe);
                    aggregatedValue.TimeStamp = timestamp;

                    sampledMetricValues.Add(aggregatedValue);
                }
                else
                {
                    sampledMetricValues.Add(new MetricValue() { TimeStamp = timestamp }); ;
                }
            }

            return new MetricDownsaplingResult()
            {
                DownsamplingValue = samplingValue,
                MetricValues = sampledMetricValues
            };
        }

        private int GetSampleValue(long startTimestamp, long endTimestamp)
        {
            // If less than 1 25 min                        => 1 second sampling
            // If less than 4 hours                         => 10 seconds sampling
            // Between 4 hours and 24 hours                 => 1 min sampling
            // Between 24 hours and a 24 hours and 5 days   => 5 min sampling
            // More than 5 days                             => 15 min sampling
            // Less than 15 days                             => 15 min sampling

            var timespan = TimeSpan.FromSeconds(endTimestamp - startTimestamp);

            if (timespan.TotalMinutes <= 25)
                return 1;
            else if (timespan.TotalHours <= 4)
                return 10;
            else if (timespan.TotalHours <= 24)
                return 60;
            else if (timespan.TotalDays <= 5)
                return 5 * 60;
            else if (timespan.TotalDays <= 15)
                return 15 * 60;
            else if (timespan.TotalDays <= 30)
                return 30 * 60; // 30 min
            else if (timespan.TotalDays <= 60)
                return 60 * 60; // 1 hour
            else throw new Exception("Time Period too big, please define new sampling value");
        }

        private MetricValue AggregateMetricsValues(IEnumerable<MetricValue> values)
        {
            double? sum;
            try
            {
                sum = values.Sum(x => x.Sum);
            }
            catch (OverflowException)
            {
                sum = double.MaxValue;
            }

            return new MetricValue() { Count = values.Sum(x => x.Count), Min = values.Min(x => x.Min), Max = values.Max(x => x.Max), Sum = sum };
        }
    }
}
