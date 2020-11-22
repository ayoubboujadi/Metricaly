using AutoMapper;
using Metricaly.Core.Entities;
using Metricaly.Infrastructure.Common.Mappings;
using System;

namespace Metricaly.Infrastructure.Dtos
{
    public class DashboardWidgetDto : IMapFrom<DashboardWidget>
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Guid WidgetId { get; set; }
        public string WidgetType { get; set; }
        public Guid? Id { get; set; }

        public void MapFrom(Profile profile)
        {
            profile.CreateMap<DashboardWidget, DashboardWidgetDto>()
               .ForMember(d => d.WidgetType, opt => opt.MapFrom(x => x.Widget == null ? null : x.Widget.Type));
        }
    }
}
