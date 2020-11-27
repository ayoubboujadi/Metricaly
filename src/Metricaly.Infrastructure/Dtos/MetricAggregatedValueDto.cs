using Metricaly.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Infrastructure.Dtos
{
    public class MetricAggregatedValueDto
    {
        public string Guid { get; set; }
        public double? Value { get; set; }
        public SamplingType SamplingType { get; set; }
        public string MetricName { get; internal set; }
        public string Namespace { get; internal set; }
    }
}
