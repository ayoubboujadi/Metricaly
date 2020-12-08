using MediatR;
using Metricaly.Infrastructure.Options.Queries.GetLiveSpanValues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metricaly.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class OptionsController : ControllerBase
    {
        private readonly IMediator mediator;

        public OptionsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("livespan")]
        public async Task<ActionResult<List<LiveSpanValueDto>>> GetLiveSpanValues()
        {
            return await mediator.Send(new GetLiveSpanValuesQuery());
        }
    }
}
