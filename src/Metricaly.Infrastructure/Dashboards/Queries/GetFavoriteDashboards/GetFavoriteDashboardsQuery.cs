using AutoMapper;
using MediatR;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Dashboards.Queries.GetFavoriteDashboards
{
    public class GetFavoriteDashboardsQuery : IRequest<List<DashboardDto>>
    {
    }

    public class GetFavoriteDashboardsQueryHandler : IRequestHandler<GetFavoriteDashboardsQuery, List<DashboardDto>>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public GetFavoriteDashboardsQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.mapper = mapper;
        }

        public async Task<List<DashboardDto>> Handle(GetFavoriteDashboardsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();

            var applicationDashboards = await (from app in context.Applications
                                               join dashboard in context.Dashboards
                                               on app.Id equals dashboard.ApplicationId
                                               where app.UserId == currentUserId && dashboard.IsFavorite
                                               select dashboard)
                                              .AsNoTracking()
                                              .ToListAsync();

            var result = new List<DashboardDto>();

            foreach (var item in applicationDashboards)
            {
                result.Add(new DashboardDto
                {
                    ApplicationId = item.ApplicationId,
                    Id = item.Id,
                    CreatedDate = item.CreatedDate,
                    Name = item.Name,
                    IsFavorite = item.IsFavorite
                });
            }

            return mapper.Map<List<DashboardDto>>(applicationDashboards);
        }
    }
}
