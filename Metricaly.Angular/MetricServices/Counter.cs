using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metricaly.Web
{
    public class Counter
    {
        public string ApplicationShortCode { get; set; }
        public string Namespace { get; set; }
        public string MetricName { get; set; }

        private string GetCounterKey(long timestamp)
        {
            return ApplicationShortCode + ":" + Namespace + ":" + MetricName + ":c:" + timestamp;
        }
    }
}
