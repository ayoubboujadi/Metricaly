using MediatR;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Applications.Commands.CreateApplication
{
    public class CreateApplicationCommand : IRequest<Guid>
    {
        public string ApplicationName { get; set; }
    }

    public class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, Guid>
    {
        private readonly ICreateApplicationService createApplicationService;
        private readonly ICurrentUserService currentUserService;

        public CreateApplicationCommandHandler(ICreateApplicationService createApplicationService, ICurrentUserService currentUserService)
        {
            this.createApplicationService = createApplicationService;
            this.currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.GetCurrentUserId();
            var application = await createApplicationService.CreateAsync(request.ApplicationName, currentUserId);

            return application.Id;
        }
    }
}
