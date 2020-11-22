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
using System.Linq;

namespace Metricaly.Infrastructure.Widgets.Commands.UpdateWidget
{
    public class UpdateWidgetCommand<T> : IRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public T WidgetData { get; set; }
    }

    public class UpdateWidgetCommandHandler<T> : IRequestHandler<UpdateWidgetCommand<T>, Unit> where T : IWidgetData
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;

        public UpdateWidgetCommandHandler(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(UpdateWidgetCommand<T> request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();

            var dbWidget = await (from widget in context.Widgets
                                  join application in context.Applications
                                  on widget.ApplicationId equals application.Id
                                  where widget.Id == request.Id && application.UserId == currentUserId
                                  select widget).FirstOrDefaultAsync();

            if (dbWidget == null)
                throw new EntityNotFoundException(nameof(Widget), request.Id);

            var widgetHandlerFactory = new WidgetsHandlerFactory();
            var widgetHandler = widgetHandlerFactory.Make(dbWidget.Type);

            dbWidget.Name = request.Name;
            dbWidget.Data = widgetHandler.GetJson(request.WidgetData);

            context.Widgets.Update(dbWidget);
            await context.SaveChangesAsync();

            return Unit.Value;
        }

    }
}
