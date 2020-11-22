using AutoMapper;
using MediatR;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Applications.Queries.ListApplications
{
    public class ListApplicationsQuery : IRequest<List<ApplicationDto>> { }

    public class ListDashboardsQueryHandler : IRequestHandler<ListApplicationsQuery, List<ApplicationDto>>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMapper mapper;

        public ListDashboardsQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService, IMapper mapper)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.mapper = mapper;
        }

        public async Task<List<ApplicationDto>> Handle(ListApplicationsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();

            var applications = await context.Applications
                .Where(a => a.UserId == currentUserId)
                .AsNoTracking()
                .ToListAsync();

            return mapper.Map<List<ApplicationDto>>(applications);
        }
    }
}
