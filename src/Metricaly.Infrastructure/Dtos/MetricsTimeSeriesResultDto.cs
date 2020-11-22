using System.Collections.Generic;

namespace Metricaly.Infrastructure.Dtos
{
    public class MetricsTimeSeriesResultDto
    {
        public int SamplingValue { get; set; }
        public int Count { get; set; }
        public List<long> Timestamps { get; set; }
        public List<MetricTimeSeriesValueDto> Values { get; set; } = new List<MetricTimeSeriesValueDto>();
    }

    public class MetricTimeSeriesValueDto
    {
        public string Guid { get; set; }
        public string MetricName { get; set; }
        public string Namespace { get; set; }
        public List<double?> Values { get; set; }
    }
}
