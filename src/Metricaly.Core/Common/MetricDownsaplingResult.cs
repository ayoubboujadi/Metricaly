using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Common
{
    public class MetricDownsaplingResult
    {
        public int DownsamplingValue { get; set; }
        public List<MetricValue> MetricValues { get; set; }
    }

}
