using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metricaly.Angular.MetricServices
{
    public class MetricRedisValue
    {
        [JsonProperty("mn")]
        public string MetricName { get; set; }

        [JsonProperty("ns")]
        public string Namespace { get; set; }

        [JsonProperty("ai")]
        public long ApplicationId { get; set; }

        [JsonProperty("ts")]
        public long Timestamp { get; set; }

        [JsonProperty("tsg")]
        public long TimestampGranulated { get; set; }

        [JsonProperty("vl")]
        public long Value { get; set; }
    }
}
