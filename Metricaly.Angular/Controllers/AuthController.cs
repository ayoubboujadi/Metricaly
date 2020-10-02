using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Identity;
using Metricaly.Web.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Metricaly.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITokenClaimsService tokenClaimsService;
        private readonly IOptionsSnapshot<JwtSettings> jwtSettings;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenClaimsService tokenClaimsService, IOptionsSnapshot<JwtSettings> jwtSettings)
        {
            this.userManager = userManager;
            this.tokenClaimsService = tokenClaimsService;
            this.jwtSettings = jwtSettings;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserSignUpResource userSignUpResource)
        {
            var user = new ApplicationUser
            {
                Email = userSignUpResource.Email,
                UserName = userSignUpResource.Email,
            };

            var userCreateResult = await userManager.CreateAsync(user, userSignUpResource.Password);

            if (userCreateResult.Succeeded)
            {
                return Ok();
            }

            return Problem(userCreateResult.Errors.First().Description, null, 500);
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(UserLoginResource userLoginResource)
        {
            var user = userManager.Users.SingleOrDefault(u => u.UserName == userLoginResource.Email);
            if (user is null)
            {
                return BadRequest("Incorrect Email and/or password.");
            }

            var userSigninResult = await userManager.CheckPasswordAsync(user, userLoginResource.Password);

            if (userSigninResult)
            {
                var token = await tokenClaimsService.GetTokenAsync(user, jwtSettings.Value.Secret);
   
                dynamic result = new
                {
                    id = user.Id,
                    email = user.Email,
                    token
                };

                return Ok(result);
            }

            return BadRequest("Incorrect Email and/or password.");
        }
    }
}
