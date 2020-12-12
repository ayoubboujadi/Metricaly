using Metricaly.Core.Common;
using System;
using System.Threading.Tasks;

namespace Metricaly.Core.Interfaces
{
    public interface IMetricsCollectionService
    {
        Task CollectSingleMetricAsync(Guid applicationId, string metricName, string metricNamespace, double value, long? timestamp = null);
        Task CollectAggregatedMetricAsync(Guid applicationId, string metricName, string metricNamespace, MetricValue metricValue);
    }
}
