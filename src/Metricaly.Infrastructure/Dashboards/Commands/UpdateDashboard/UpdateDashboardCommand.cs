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

namespace Metricaly.Infrastructure.Dashboards.Commands.UpdateDashboard
{
    public class UpdateDashboardCommand : IRequest
    {
        public Guid DashboardId { get; set; }
        public Guid ApplicationId { get; set; }
        public List<DashboardWidget> DashboardWidgets { get; set; }
    }

    public class UpdateDashboardCommandHandler : IRequestHandler<UpdateDashboardCommand>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;

        public UpdateDashboardCommandHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(UpdateDashboardCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();

            var dbDashboard = await GetDashboard(request.DashboardId, request.ApplicationId, currentUserId);

            if (dbDashboard == null)
                throw new EntityNotFoundException(nameof(Dashboard), request.DashboardId);

            // Validate and remove the Widgets that don't belong to this dashboard
            request.DashboardWidgets = await ValidateWidgets(request.DashboardWidgets, request.ApplicationId);

            // Remove DashboardWidgets that are not present (removed)
            dbDashboard.DashboardWidgets = dbDashboard.DashboardWidgets
                .Where(oldWidget => request.DashboardWidgets.Exists(newWidget => newWidget.Id == oldWidget.Id)).ToList();

            foreach (var dashboardWidget in request.DashboardWidgets)
            {
                if (dashboardWidget.Id == null || !dbDashboard.DashboardWidgets.Exists(x => x.Id == dashboardWidget.Id))
                {
                    // Insert DashboardWidget
                    dbDashboard.DashboardWidgets.Add(new DashboardWidget
                    {
                        Height = dashboardWidget.Height,
                        Width = dashboardWidget.Width,
                        X = dashboardWidget.X,
                        Y = dashboardWidget.Y,
                        WidgetId = dashboardWidget.WidgetId,
                        DashboardId = dbDashboard.Id,
                    });
                }
                else
                {
                    // Update DashboardWidget
                    var dbWidget = dbDashboard.DashboardWidgets.FirstOrDefault(x => x.Id == dashboardWidget.Id);
                    dbWidget.Height = dashboardWidget.Height;
                    dbWidget.Width = dashboardWidget.Width;
                    dbWidget.X = dashboardWidget.X;
                    dbWidget.Y = dashboardWidget.Y;
                }
            }

            context.Dashboards.Update(dbDashboard);

            await context.SaveChangesAsync();

            return Unit.Value;
        }

        private async Task<List<DashboardWidget>> ValidateWidgets(List<DashboardWidget> dashboardWidgets, Guid applicationId)
        {
            var widgetsIdsToValidate = dashboardWidgets.Select(x => x.WidgetId).Distinct().ToList();

            // Check if the widgets are part of the same application
            var allowedWidgetsIds = await (from application in context.Applications
                          join widget in context.Widgets
                          on application.Id equals widget.ApplicationId
                          where widgetsIdsToValidate.Contains(widget.Id)
                              && application.Id == applicationId
                          select widget.Id)
                                .ToListAsync();

            // Remove the dashboardWidgets that have an invalid widget
            dashboardWidgets = dashboardWidgets
                .Where(x => allowedWidgetsIds.Contains(x.WidgetId)).ToList();

            return dashboardWidgets;
        }

        private async Task<Dashboard> GetDashboard(Guid dashboardId, Guid applicationId, string userId)
        {
            return await (from dashboard in context.Dashboards
                                    join application in context.Applications
                                    on dashboard.ApplicationId equals application.Id
                                    where dashboard.Id == dashboardId && dashboard.ApplicationId == applicationId
                                          && application.UserId == userId
                          select dashboard)
                      .Include(dashboard => dashboard.DashboardWidgets)
                      .FirstOrDefaultAsync();
        }
    }

}
