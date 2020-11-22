using MediatR;
using Metricaly.Infrastructure.Applications.Commands.CreateApplication;
using Metricaly.Infrastructure.Applications.Queries.ListApplications;
using Metricaly.Infrastructure.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metricaly.Angular.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationController : ControllerBase
    {
        private readonly IMediator mediator;

        public ApplicationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<ApplicationDto>>> List()
        {
            return await mediator.Send(new ListApplicationsQuery());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApplicationCommand createApplicationCommand)
        {
            var id = await mediator.Send(createApplicationCommand);
            return Ok(new { applicationId = id });
        }
    }
}
