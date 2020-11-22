using Metricaly.Core.Common;
using Metricaly.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Metricaly.Core.Interfaces
{
    public interface IMetricsRetriever
    {
        Task<MetricValue[]> QueryAsync(Guid applicationId, string metricName, string metricNamespace, TimePeriod timePeriod);
        Task<MetricValue[]> QueryAsync(Metric metric, TimePeriod timePeriod);
    }
}