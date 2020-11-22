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
using Metricaly.Infrastructure.Common.Exceptions;

namespace Metricaly.Infrastructure.Dashboards.Queries.GetDashboard
{
    public class GetDashboardQuery : IRequest<DashboardDetailsVm>
    {
        public Guid DashboardId { get; set; }
    }

    public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDetailsVm>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public GetDashboardQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.mapper = mapper;
        }

        public async Task<DashboardDetailsVm> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();

            var dbDashboard = await (from application in context.Applications
                                join dashboard in context.Dashboards
                                on application.Id equals dashboard.ApplicationId
                                where dashboard.Id == request.DashboardId && application.UserId == currentUserId
                                select dashboard)
                                .Include(x => x.DashboardWidgets)
                                .ThenInclude(dw => dw.Widget)
                                .FirstOrDefaultAsync();

            if (dbDashboard == null)
                throw new EntityNotFoundException(nameof(Dashboard), request.DashboardId);

            return new DashboardDetailsVm
            {
                Dashboard = mapper.Map<DashboardDto>(dbDashboard),
                DashboardWidgets = mapper.Map<IList<DashboardWidgetDto>>(dbDashboard.DashboardWidgets)
            };
        }
    }
}
