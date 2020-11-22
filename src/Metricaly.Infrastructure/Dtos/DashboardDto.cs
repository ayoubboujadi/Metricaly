using AutoMapper;
using Metricaly.Core.Entities;
using Metricaly.Infrastructure.Common.Mappings;
using System;

namespace Metricaly.Infrastructure.Dtos
{
    public class DashboardDto : IMapFrom<Dashboard>
    {
        public Guid ApplicationId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsFavorite { get; set; }
        public int DashboardWidgetsCount { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Dashboard, DashboardDto>()
                .ForMember(d => d.DashboardWidgetsCount, opt => opt.MapFrom(s => s.DashboardWidgets.Count));
        }
    }
}
