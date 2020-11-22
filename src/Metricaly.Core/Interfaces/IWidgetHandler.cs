using Metricaly.Core.Widgets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Interfaces
{
    public interface IWidgetHandler
    {
        string GetDefaultJson();
        string GetJson(IWidgetData widgetData);
        IWidgetData FromJson(string jsonWidgetData);
    }
}
