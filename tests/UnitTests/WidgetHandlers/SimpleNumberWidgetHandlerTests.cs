using FluentAssertions;
using Metricaly.Core.Interfaces;
using Metricaly.Core.Widgets.SimpleNumber;
using Metricaly.Infrastructure.WidgetHandlers;
using NUnit.Framework;

namespace Infrastructure.UnitTests.WidgetHandlers
{
    public class SimpleNumberWidgetHandlerTests
    {
        private readonly IWidgetHandler widgetHandler = new SimpleNumberWidgetHandler();

        [Test]
        public void ShouldReturnJsonForDefaultSimpleNumberWidgetData()
        {
            var widgetJson = widgetHandler.GetDefaultJson();

            widgetJson.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void ShouldParseJsonToSimpleNumberWidgetData()
        {
            var json = widgetHandler.GetJson(new SimpleNumberWidget
            {
                Title = "MyTitle"
            });

            var widget = widgetHandler.FromJson(json) as SimpleNumberWidget;

            widget.Should().NotBeNull();
            widget.Title.Should().Be("MyTitle");
        }
    }
}
