using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Widgets
{
    public enum WidgetType
    {
        LineChart
    }

    public interface IWidgetData
    {
    }

    public class WidgetData : IWidgetData
    {
        public string Title { get; set; }
    }
}
