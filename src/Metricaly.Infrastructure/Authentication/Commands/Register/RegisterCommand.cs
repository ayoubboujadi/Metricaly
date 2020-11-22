using MediatR;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Common.Exceptions;
using Metricaly.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Metricaly.Infrastructure.Authentication.Commands.Register
{
    public class RegisterCommand : IRequest<Unit>
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public RegisterCommandHandler(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
            };

            var userCreateResult = await userManager.CreateAsync(user, request.Password);

            if (userCreateResult.Succeeded)
            {
                return Unit.Value;
            }

            var errors = userCreateResult.Errors.Select(x => (x.Code, x.Description)).ToDictionary(x => x.Code, x => x.Description);

            throw new ApiException("An error occured while trying to create your account.", errors: errors);
        }
    }
}
