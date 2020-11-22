using MediatR;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Common.Exceptions;
using Metricaly.Infrastructure.Common.Models;
using Metricaly.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Authentication.Commands.Authenticate
{
    public class AuthenticatedDto
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
    }

    public class AuthenticateCommand : IRequest<AuthenticatedDto>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthenticatedCommandHandler : IRequestHandler<AuthenticateCommand, AuthenticatedDto>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITokenClaimsService tokenClaimsService;
        private readonly JwtSettings jwtSettings;

        public AuthenticatedCommandHandler(UserManager<ApplicationUser> userManager, ITokenClaimsService tokenClaimsService, IOptions<JwtSettings> options)
        {
            this.userManager = userManager;
            this.tokenClaimsService = tokenClaimsService;
            this.jwtSettings = options.Value;
        }

        public async Task<AuthenticatedDto> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
        {
            var user = userManager.Users.SingleOrDefault(u => u.UserName == request.Email);
            if (user is null)
            {
                throw new ApiException("Incorrect Email and/or password.");
            }

            var userSigninResult = await userManager.CheckPasswordAsync(user, request.Password);

            if (userSigninResult)
            {
                var token = await tokenClaimsService.GetTokenAsync(user, jwtSettings.Secret);

                return new AuthenticatedDto
                {
                    Token = token,
                    UserId = user.Id,
                    UserEmail = user.Email
                };
            }

            throw new ApiException("Incorrect Email and/or password.");
        }
    }

}
