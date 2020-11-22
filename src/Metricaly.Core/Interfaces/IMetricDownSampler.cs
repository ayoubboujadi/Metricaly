using Metricaly.Core.Common;

namespace Metricaly.Core.Interfaces
{
    public interface IMetricDownSampler
    {
        MetricDownsaplingResult DownSample(MetricValue[] metricValues, int preferedSamplingValue, long startTimestamp, long endTimestamp);
    }
}