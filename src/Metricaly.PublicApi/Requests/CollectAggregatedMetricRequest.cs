using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metricaly.PublicApi.Requests
{
    public class CollectAggregatedMetricRequest
    {
        public string MetricName { get; set; }
        public string MetricNamespace { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public double Sum { get; set; }
        public long SamplesCount { get; set; }
        public long? Timestamp { get; set; }
    }
}
