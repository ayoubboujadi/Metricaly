using MediatR;
using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Core.Widgets;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Common.Exceptions;
using Metricaly.Infrastructure.WidgetHandlers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Widgets.Commands.CreateWidget
{
    public class CreateWidgetCommand : IRequest<Guid>
    {
        public Guid ApplicationId { get; set; }
        public string Name { get; set; }
        public WidgetType WidgetType { get; set; }
    }

    public class CreateWidgetCommandHandler : IRequestHandler<CreateWidgetCommand, Guid>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;

        public CreateWidgetCommandHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateWidgetCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();

            var application = await context.Applications
                .FirstOrDefaultAsync(app => app.Id == request.ApplicationId && app.UserId == currentUserId);

            if (application == null)
                throw new EntityNotFoundException(nameof(Application), request.ApplicationId);

            var widgetHandlerFactory = new WidgetsHandlerFactory();
            var widgetHandler = widgetHandlerFactory.Make(request.WidgetType);

            var widget = new Widget(request.Name, application.Id)
            {
                Type = request.WidgetType.ToString(),
                Data = widgetHandler.GetDefaultJson()
            };

            await context.Widgets.AddAsync(widget);
            await context.SaveChangesAsync();

            return widget.Id;
        }
    }
}
