using Metricaly.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Metricaly.Core.Interfaces
{
    public interface IMetricsClient
    {
        Task<MetricValue[]> QueryMetricsAsync(string metricKey, long startTimestamp, long endTimestamp);
        MetricValue[] QueryMetrics(string metricKey, long startTimestamp, long endTimestamp);

        Task<bool> CollectMetricAsync(string metricKey, MetricValue value);
        bool CollectMetric(string metricKey, MetricValue value);
    }
}
