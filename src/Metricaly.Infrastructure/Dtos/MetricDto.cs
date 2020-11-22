using Metricaly.Core.Entities;
using Metricaly.Infrastructure.Common.Mappings;
using System;

namespace Metricaly.Infrastructure.Dtos
{
    public class MetricDto : IMapFrom<Metric>
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid Id { get; set; }
    }
}
