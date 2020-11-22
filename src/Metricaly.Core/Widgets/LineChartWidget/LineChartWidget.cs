using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Widgets.LineChartWidget
{
    public class LineChartWidget : WidgetData
    {
        public int SamplingTime { get; set; } = 1; // 1 Seconds
        public List<LineChartPlottedMetric> PlottedMetrics { get; set; } = new List<LineChartPlottedMetric>();
        public LineChartWidgetSettings WidgetSettings { get; set; } = new LineChartWidgetSettings();
    }

    public class LineChartWidgetSettings
    {
        public string Title { get; set; }
        public string LegendPosition { get; set; }
        public bool DisplayLegend { get; set; }
        public bool SmoothLines { get; set; }
        public bool Filled { get; set; }
        public LineChartAxisSettings XAxisSettings { get; set; } = new LineChartAxisSettings();
        public LineChartAxisSettings YLeftAxisSettings { get; set; } = new LineChartAxisSettings();
        public LineChartAxisSettings YRightAxisSettings { get; set; } = new LineChartAxisSettings();
    }

    public class LineChartPlottedMetric
    {
        public int MetricId { get; set; }
        public string Guid { get; set; }
        public string Label { get; set; }
        public string Color { get; set; }
        public string MetricName { get; set; }
        public string Namespace { get; set; }
        public string YAxis { get; set; }
        public string SamplingType { get; set; }
    }

    public class LineChartAxisSettings
    {
        public string Label { get; set; }
        public bool DisplayLabel { get; set; }
        public bool DisplayGridLines { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }

}
