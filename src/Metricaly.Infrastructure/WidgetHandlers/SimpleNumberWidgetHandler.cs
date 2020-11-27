using Metricaly.Core.Interfaces;
using Metricaly.Core.Widgets;
using Metricaly.Core.Widgets.SimpleNumber;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Metricaly.Infrastructure.WidgetHandlers
{
    public class SimpleNumberWidgetHandler : IWidgetHandler
    {
        public IWidgetData FromJson(string jsonWidgetData)
        {
            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Deserialize<SimpleNumberWidget>(jsonWidgetData, options);
        }

        public string GetDefaultJson()
        {
            return ParseWidgetToJson(new SimpleNumberWidget());
        }

        public string GetJson(IWidgetData widgetData)
        {
            var simpleNumberWidget = widgetData as SimpleNumberWidget;

            return ParseWidgetToJson(simpleNumberWidget);
        }

        private string ParseWidgetToJson(SimpleNumberWidget simpleNumberWidget)
        {
            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Serialize(simpleNumberWidget, options);
        }
    }
}
