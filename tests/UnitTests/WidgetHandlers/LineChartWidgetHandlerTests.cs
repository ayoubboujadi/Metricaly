using FluentAssertions;
using Metricaly.Core.Interfaces;
using Metricaly.Core.Widgets.LineChartWidget;
using Metricaly.Infrastructure.WidgetHandlers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.UnitTests.WidgetHandlers
{
    public class LineChartWidgetHandlerTests
    {
        private readonly IWidgetHandler lineChartWidgetHandler = new LineChartWidgetHandler();

        [Test]
        public void ShouldReturnJsonForDefaultLineChartWidgetData()
        {
            var widgetJson = lineChartWidgetHandler.GetDefaultJson();

            widgetJson.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void ShouldParseJsonToLineChartWidgetData()
        {
            var json = lineChartWidgetHandler.GetJson(new LineChartWidget
            {
                SamplingTime = 60,
                Title = "MyTitle"
            });

            var widget = lineChartWidgetHandler.FromJson(json) as LineChartWidget;

            widget.Should().NotBeNull();
            widget.SamplingTime.Should().Equals(60);
            widget.Title.Should().Be("MyTitle");
        }
    }
}
