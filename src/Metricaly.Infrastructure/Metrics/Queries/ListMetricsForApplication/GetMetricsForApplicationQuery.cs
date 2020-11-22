using AutoMapper;
using MediatR;
using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Dtos;
using Metricaly.Infrastructure.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Metrics.Queries.GetMetricsForApplication
{
    public class GetMetricsForApplicationQuery : IRequest<List<MetricDto>>
    {
        public Guid ApplicationId { get; set; }
    }

    public class GetMetricsForApplicationQueryHandler : IRequestHandler<GetMetricsForApplicationQuery, List<MetricDto>>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public GetMetricsForApplicationQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.mapper = mapper;
        }

        public async Task<List<MetricDto>> Handle(GetMetricsForApplicationQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();

            var application = await context.Applications
                .Where(application => application.Id == request.ApplicationId && application.UserId == currentUserId)
                .Include(application => application.Metrics)
                .FirstOrDefaultAsync();

            if (application == null)
                throw new EntityNotFoundException(nameof(Application), request.ApplicationId);

            return mapper.Map<List<MetricDto>>(application.Metrics);
        }
    }
}
