using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metricaly.PublicApi.Requests
{
    public class CollectSingleMetricRequest
    {
        public string MetricName { get; set; }
        public string MetricNamespace { get; set; }
        public double Value { get; set; }
        public long? Timestamp { get; set; }
    }
}
