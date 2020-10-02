using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace Metricaly.Infrastructure.Identity
{
    public class IdentityTokenClaimService : ITokenClaimsService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityTokenClaimService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GetTokenAsync(IApplicationUser applicationUser,  string jwtSecretKey)
        {
            ApplicationUser user = applicationUser as ApplicationUser;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecretKey);
            //var user = await _userManager.FindByEmailAsync(applicationUser.Email);
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim> { new Claim(ClaimTypes.Email, applicationUser.Email), new Claim(ClaimTypes.NameIdentifier, user.Id) };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
