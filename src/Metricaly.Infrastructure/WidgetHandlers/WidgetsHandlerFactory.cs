using Metricaly.Core.Interfaces;
using Metricaly.Core.Widgets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Infrastructure.WidgetHandlers
{
    public class WidgetsHandlerFactory
    {
        public WidgetsHandlerFactory()
        {
        }

        public IWidgetHandler Make(WidgetType type)
        {
            switch (type)
            {
                case WidgetType.LineChart:
                    return new LineChartWidgetHandler();
                default:
                    return null;
            }
        }

        public IWidgetHandler Make(string type)
        {
            try
            {
                var typeEnum = (WidgetType)Enum.Parse(typeof(WidgetType), type.Replace("-", "").Trim(), true);
                return Make(typeEnum);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
