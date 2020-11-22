using Metricaly.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using FluentAssertions;
using Metricaly.Infrastructure.Services;
using Metricaly.Core.Common;

namespace Infrastructure.UnitTests.Services
{
    public class MetricDownSamplerTests
    {
        private readonly IMetricDownSampler metricDownSampler;
        public MetricDownSamplerTests()
        {
            metricDownSampler = new MetricDownSampler();
        }

        private MetricValue[] GetMetricValues()
        {
            return new MetricValue[]
            {
                new MetricValue(){Count = 1, Max = 50, Min = 10, Sum = 100, TimeStamp = 1000},
                new MetricValue(){Count = 1, Max = 50, Min = 10, Sum = 100, TimeStamp = 1001},
                new MetricValue(){Count = 1, Max = 50, Min = 10, Sum = 100, TimeStamp = 1002},
                new MetricValue(){Count = 1, Max = 50, Min = 10, Sum = 100, TimeStamp = 1003},
                new MetricValue(){Count = 1, Max = 50, Min = 10, Sum = 100, TimeStamp = 1004},
            };
        }

        [Test]
        public void ShouldDownSampleToSingleResult()
        {
            var metricValues = GetMetricValues();

            var downsaplingResult = metricDownSampler.DownSample(metricValues, 10, 1000, 1009);

            downsaplingResult.DownsamplingValue.Should().Be(10);
            downsaplingResult.MetricValues.Should().HaveCount(1);
            downsaplingResult.MetricValues[0].Count.Should().Be(5);
            downsaplingResult.MetricValues[0].Min.Should().Be(10);
            downsaplingResult.MetricValues[0].Max.Should().Be(50);
            downsaplingResult.MetricValues[0].Sum.Should().Be(500);
            downsaplingResult.MetricValues[0].TimeStamp.Should().Be(1000);
        }

        [Test]
        public void ShouldDownSampleToTwoResult()
        {
            var metricValues = GetMetricValues();

            var downsaplingResult = metricDownSampler.DownSample(metricValues, 10, 1000, 1010);

            downsaplingResult.DownsamplingValue.Should().Be(10);
            downsaplingResult.MetricValues.Should().HaveCount(2);
            downsaplingResult.MetricValues[1].Count.Should().Be(null);
            downsaplingResult.MetricValues[1].Min.Should().Be(null);
            downsaplingResult.MetricValues[1].Max.Should().Be(null);
            downsaplingResult.MetricValues[1].Sum.Should().Be(null);
            downsaplingResult.MetricValues[1].TimeStamp.Should().Be(1010);
        }

        [Test]
        public void ShouldDownSampleToEmptyResult()
        {
            var metricValues = GetMetricValues();

            var downsaplingResult = metricDownSampler.DownSample(metricValues, 10, 1050, 1059);

            downsaplingResult.MetricValues.Should().HaveCount(1);
            downsaplingResult.MetricValues[0].Count.Should().Be(null);
            downsaplingResult.MetricValues[0].Min.Should().Be(null);
            downsaplingResult.MetricValues[0].Max.Should().Be(null);
            downsaplingResult.MetricValues[0].Sum.Should().Be(null);
            downsaplingResult.MetricValues[0].TimeStamp.Should().Be(1050);
        }
        
        [Test]
        public void ShouldDownSampleToPreferedSamplingTimeValue()
        {
            var metricValues = GetMetricValues();

            var downsaplingResult = metricDownSampler.DownSample(metricValues, 300, 900, 1400);

            downsaplingResult.DownsamplingValue.Should().Be(300);
            downsaplingResult.MetricValues.Should().HaveCount(2);
            downsaplingResult.MetricValues[0].TimeStamp.Should().Be(900);
            downsaplingResult.MetricValues[1].TimeStamp.Should().Be(1200);
        }

        [Test]
        public void ShouldNotDownSampleToPreferedOneSecondSamplingTimeValue()
        {
            var metricValues = new MetricValue[10000];
            for (int i = 0; i < 10000; i++)
            {
                metricValues[i] = new MetricValue() { Count = 1, Max = 50, Min = 10, Sum = 100, TimeStamp = i };
            }

            var downsamplingResult = metricDownSampler.DownSample(metricValues, 1, 0, 9999);

            downsamplingResult.DownsamplingValue.Should().Be(10);
            downsamplingResult.MetricValues.Should().HaveCount(1000);
            downsamplingResult.MetricValues[0].TimeStamp.Should().Be(0);
            downsamplingResult.MetricValues[0].Max.Should().Be(50);
            downsamplingResult.MetricValues[0].Min.Should().Be(10);
            downsamplingResult.MetricValues[0].Sum.Should().Be(1000);
            downsamplingResult.MetricValues[0].Count.Should().Be(10);
        }

        [Test]
        public void ShouldNotDownSampleToPreferedTenSecondsSamplingTimeValue()
        {
            var downsamplingResult = metricDownSampler.DownSample(new MetricValue[] { }, 10, 0, 20000);

            downsamplingResult.DownsamplingValue.Should().Be(60);
            downsamplingResult.MetricValues.Should().HaveCount(334);
        }
    }
}
