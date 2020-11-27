using Metricaly.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Widgets.SimpleNumber
{
    public class SimpleNumberWidget : WidgetData
    {
        public List<SimpleNumberPlottedMetric> PlottedMetrics { get; set; } = new List<SimpleNumberPlottedMetric>();
    }

    public class SimpleNumberPlottedMetric
    {
        public string MetricId { get; set; }
        public string Guid { get; set; }
        public string Label { get; set; }
        public string Unit { get; set; }
        public string Color { get; set; }
        public string MetricName { get; set; }
        public string Namespace { get; set; }
        public SamplingType SamplingType { get; set; }
    }
}
