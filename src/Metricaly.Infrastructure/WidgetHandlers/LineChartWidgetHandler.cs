using Metricaly.Core.Interfaces;
using Metricaly.Core.Widgets;
using Metricaly.Core.Widgets.LineChartWidget;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Metricaly.Infrastructure.WidgetHandlers
{
    public class LineChartWidgetHandler : IWidgetHandler
    {
        public IWidgetData FromJson(string jsonWidgetData)
        {
            //return JsonConvert.DeserializeObject<LineChartWidget>(jsonWidgetData);

            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Deserialize<LineChartWidget>(jsonWidgetData, options);
        }

        public string GetDefaultJson()
        {
            return ParseWidgetToJson(new LineChartWidget());
        }

        public string GetJson(IWidgetData widgetData)
        {
            var lineChartWidget = widgetData as LineChartWidget;

            return ParseWidgetToJson(lineChartWidget);
        }

        private string ParseWidgetToJson(LineChartWidget lineChartWidget)
        {
            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Serialize(lineChartWidget, options);
        }
    }
}
