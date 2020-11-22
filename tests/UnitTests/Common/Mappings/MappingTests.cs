using AutoMapper;
using Metricaly.Core.Entities;
using Metricaly.Infrastructure.Common.Mappings;
using Metricaly.Infrastructure.Dtos;
using NUnit.Framework;
using System;
using FluentAssertions;

namespace Infrastructure.UnitTests.Common.Mappings
{
    public class MappingTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Test]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [Test]
        [TestCase(typeof(Application), typeof(ApplicationDto))]
        [TestCase(typeof(Widget), typeof(WidgetDto))]
        [TestCase(typeof(Dashboard), typeof(DashboardDto))]
        [TestCase(typeof(DashboardWidget), typeof(DashboardWidgetDto))]
        [TestCase(typeof(Metric), typeof(MetricDto))]
        public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
        {
            var instance = Activator.CreateInstance(source);

            _mapper.Map(instance, source, destination);
        }
    }
}
