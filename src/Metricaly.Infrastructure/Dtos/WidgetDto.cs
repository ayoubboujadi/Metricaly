using Metricaly.Core.Entities;
using Metricaly.Core.Widgets;
using Metricaly.Infrastructure.Common.Mappings;
using System;

namespace Metricaly.Infrastructure.Dtos
{
    public class WidgetDto : IMapFrom<Widget>
    {
        public Guid ApplicationId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
