using MediatR;
using Metricaly.Infrastructure.Authentication.Commands.Authenticate;
using Metricaly.Infrastructure.Authentication.Commands.Register;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Metricaly.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;

        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(RegisterCommand registerCommand)
        {
            await mediator.Send(registerCommand);
            return NoContent();
        }

        [HttpPost("signin")]
        public async Task<ActionResult<AuthenticatedDto>> SignIn(AuthenticateCommand authenticateCommand)
        {
            return await mediator.Send(authenticateCommand);
        }
    }
}
