using MediatR;
using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Metricaly.Infrastructure.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Metricaly.Infrastructure.Dtos;
using AutoMapper;

namespace Metricaly.Infrastructure.Widgets.Queries.GetWidgetForApplication
{
    public class GetWidgetForApplicationQuery : IRequest<List<WidgetDto>>
    {
        public Guid ApplicationId { get; set; }
    }

    public class GetWidgetForApplicationQueryHandler : IRequestHandler<GetWidgetForApplicationQuery, List<WidgetDto>>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public GetWidgetForApplicationQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.mapper = mapper;
        }

        public async Task<List<WidgetDto>> Handle(GetWidgetForApplicationQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();

            var application = await context.Applications
                .Where(application => application.Id == request.ApplicationId && application.UserId == currentUserId)
                .Include(application => application.Widgets)
                .FirstOrDefaultAsync();

            if (application == null)
                throw new EntityNotFoundException(nameof(Application), request.ApplicationId);

            return mapper.Map<List<WidgetDto>>(application.Widgets);
        }
    }
}
