using MediatR;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Metricaly.Infrastructure.Common.Exceptions;
using Metricaly.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Metricaly.Infrastructure.Dtos;
using AutoMapper;

namespace Metricaly.Infrastructure.Widgets.Queries.GetWidgetType
{
    public class GetWidgetDetails : IRequest<WidgetDto>
    {
        public Guid WidgetId { get; set; }
    }

    public class GetWidgetDetailsQueryHandler : IRequestHandler<GetWidgetDetails, WidgetDto>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public GetWidgetDetailsQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.mapper = mapper;
        }

        public async Task<WidgetDto> Handle(GetWidgetDetails request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();

            var dbWidget = await (from application in context.Applications
                                join widget in context.Widgets
                                on application.Id equals widget.ApplicationId
                                where widget.Id == widget.Id && application.UserId == currentUserId
                                select widget)
                                .FirstOrDefaultAsync();

            if (dbWidget == null)
                throw new EntityNotFoundException(nameof(Widget), request.WidgetId);

            return mapper.Map<WidgetDto>(dbWidget);
        }
    }
}
