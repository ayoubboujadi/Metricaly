using MediatR;
using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Metricaly.Infrastructure.Dashboards.Commands.DeleteDashboard
{
    public class DeleteDashboardCommand : IRequest
    {
        public Guid DashboardId { get; set; }
    }

    public class DeleteDashboardCommandHandler : IRequestHandler<DeleteDashboardCommand>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;

        public DeleteDashboardCommandHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeleteDashboardCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();

            var dashboardDb = await (from application in context.Applications
                                     join dashboard in context.Dashboards
                                     on application.Id equals dashboard.ApplicationId
                                     where dashboard.Id == request.DashboardId && application.UserId == currentUserId
                                     select dashboard)
                                .FirstOrDefaultAsync();

            if (dashboardDb == null)
                throw new EntityNotFoundException(nameof(Dashboard), request.DashboardId);

            context.Dashboards.Remove(dashboardDb);

            await context.SaveChangesAsync();

            return Unit.Value;
        }

    }

}
