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

namespace Metricaly.Infrastructure.Dashboards.Commands.FavoriteDashboard
{
    public class FavoriteDashboardCommand : IRequest<bool>
    {
        public Guid DashboardId { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class FavoriteDashboardCommandHandler : IRequestHandler<FavoriteDashboardCommand, bool>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;

        public FavoriteDashboardCommandHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<bool> Handle(FavoriteDashboardCommand request, CancellationToken cancellationToken)
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

            dbDashboard.IsFavorite = request.IsFavorite;

            context.Update(dbDashboard);
            await context.SaveChangesAsync();

            return dbDashboard.IsFavorite;
        }
    }

}
