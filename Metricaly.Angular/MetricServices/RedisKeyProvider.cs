using Metricaly.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metricaly.Angular.MetricServices
{
    public static class RedisKeyProvider
    {
        //public static string GetCounterKey(string applicationShortCode, string @namespace, string metricName, long timestamp)
        //{
        //    return applicationShortCode + ":" + @namespace + ":" + metricName + ":c:" + timestamp;
        //}

        public static string NewlyAddedCountersSortedSetName = "newlyadded";

        public static string GetCounterKey(long metricId, long timestamp)
        {
            return metricId + ":c:" + timestamp;
        }


        public static string GetMetricSortedSetKey(Metric metric)
        {
            return GetMetricSortedSetKey(metric.Id.ToString());
        }

        public static string GetMetricSortedSetKey(string metricId)
        {
            return metricId + ":metrics";
        }
    }
}
