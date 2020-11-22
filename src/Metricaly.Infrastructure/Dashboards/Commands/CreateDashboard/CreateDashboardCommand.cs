using MediatR;
using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Dashboards.Commands.CreateDashboard
{
    public class CreateDashboardCommand : IRequest<Guid>
    {
        public Guid ApplicationId { get; set; }
        public string DashboardName { get; set; }
    }

    public class CreateDashboardCommandHandler : IRequestHandler<CreateDashboardCommand, Guid>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;

        public CreateDashboardCommandHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateDashboardCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();

            var application = await context.Applications
                .FirstOrDefaultAsync(app => app.Id == request.ApplicationId && app.UserId == currentUserId);

            if (application == null)
                throw new EntityNotFoundException(nameof(Application), request.ApplicationId);

            var dashboard = new Dashboard(request.DashboardName, request.ApplicationId);

            await context.Dashboards.AddAsync(dashboard);
            await context.SaveChangesAsync();

            return dashboard.Id;
        }
    }

}
