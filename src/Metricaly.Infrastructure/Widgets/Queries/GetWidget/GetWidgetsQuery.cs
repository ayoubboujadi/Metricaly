using MediatR;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Metricaly.Infrastructure.WidgetHandlers;
using Metricaly.Infrastructure.Dtos;
using AutoMapper;
using Metricaly.Core.Widgets;

namespace Metricaly.Infrastructure.Widgets.Queries.GetWidget
{
    public class GetWidgetsQuery<T> : IRequest<List<WidgetDetailsVm<T>>> where T : IWidgetData
    {
        public List<Guid> WidgetsIds { get; set; }
    }

    public class GetWidgetsQueryHandler<T> : IRequestHandler<GetWidgetsQuery<T>, List<WidgetDetailsVm<T>>> where T : IWidgetData
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public GetWidgetsQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.mapper = mapper;
        }

        public async Task<List<WidgetDetailsVm<T>>> Handle(GetWidgetsQuery<T> request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();

            var dbWidgets = await (from application in context.Applications
                                   join widget in context.Widgets
                                   on application.Id equals widget.ApplicationId
                                   where request.WidgetsIds.Contains(widget.Id) && application.UserId == currentUserId
                                   select widget)
                                .AsNoTracking()
                                .ToListAsync();


            var widgetHandlerFactory = new WidgetsHandlerFactory();
            var result = new List<WidgetDetailsVm<T>>();
            foreach (var dbWidget in dbWidgets)
            {
                var widgetHandler = widgetHandlerFactory.Make(dbWidget.Type);

                var widgetData = new WidgetDetailsVm<T>
                {
                    WidgetData = (T)widgetHandler.FromJson(dbWidget.Data),
                    Widget = mapper.Map<WidgetDto>(dbWidget)
                };

                result.Add(widgetData);
            }

            return result;
        }
    }
}
