using System;
using System.Threading.Tasks;

namespace Metricaly.Core.Interfaces
{
    public interface IMetricsCollectionService
    {
        Task CollectSingleAggregateAsync(Guid applicationId, string metricName, string metricNamespace, double value);
    }
}
