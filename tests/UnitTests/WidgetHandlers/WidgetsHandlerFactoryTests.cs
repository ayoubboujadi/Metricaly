using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Metricaly.Core.Widgets;
using Metricaly.Infrastructure.WidgetHandlers;
using NUnit.Framework;

namespace Infrastructure.UnitTests.WidgetHandlers
{
    public class WidgetsHandlerFactoryTests
    {
        private readonly WidgetsHandlerFactory factory = new WidgetsHandlerFactory();

        [Theory]
        public void ShouldMakeWidgetHandlerGivenAllWidgetTypesAsEnum(WidgetType widgetType)
        {
            var widgetHandler = factory.Make(widgetType);

            widgetHandler.Should().NotBeNull();
        }

        [Test]
        [TestCase("LineChart")]
        [TestCase("Line-Chart")]
        [TestCase("line-chart")]
        [TestCase("linechart")]
        [TestCase("lineChart")]
        public void ShouldMakeWidgetHandlerGivenAllWidgetTypesAsString(string widgetType)
        {
            var widgetHandler = factory.Make(widgetType);

            widgetHandler.Should().NotBeNull();
        }

        [Test]
        public void ShouldReturnNullForUnknownWidgetType()
        {
            var widgetHandler = factory.Make("SomeNonExistentWidgetType");

            widgetHandler.Should().BeNull();
        }
    }
}
