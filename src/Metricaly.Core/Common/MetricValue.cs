using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Common
{
    public class MetricValue
    {
        public long TimeStamp { get; set; }
        public double? Count { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public double? Sum { get; set; }
    }
}
