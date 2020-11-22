using AutoMapper;
using MediatR;
using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Dashboards.Queries.ListDashboards
{
    public class ListDashboardsQuery : IRequest<List<DashboardDto>>
    {
        public Guid ApplicationId { get; set; }
    }

    public class ListDashboardsQueryHandler : IRequestHandler<ListDashboardsQuery, List<DashboardDto>>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public ListDashboardsQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.mapper = mapper;
        }

        public async Task<List<DashboardDto>> Handle(ListDashboardsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();

            var dbDashboards = await (from app in context.Applications
                                    join dashboard in context.Dashboards
                                    on app.Id equals dashboard.ApplicationId
                                    where app.UserId == currentUserId && app.Id == request.ApplicationId
                                    select new { dashboard, count = dashboard.DashboardWidgets.Count })
                                    .AsNoTracking()
                                    .ToListAsync();

            var result = new List<DashboardDto>();
            foreach (var dbDashboard in dbDashboards)
            {
                var dashboard = mapper.Map<DashboardDto>(dbDashboard.dashboard);
                dashboard.DashboardWidgetsCount = dbDashboard.count;
                result.Add(dashboard);
            }

            return result;
        }
    }
}
