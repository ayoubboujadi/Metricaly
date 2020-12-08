using MediatR;
using Metricaly.Infrastructure.Dtos;
using Metricaly.Infrastructure.Widgets.Commands.CreateWidget;
using Metricaly.Infrastructure.Widgets.Commands.UpdateWidget;
using Metricaly.Infrastructure.Widgets.Queries.GetWidgetForApplication;
using Metricaly.Infrastructure.Widgets.Queries.GetWidgetType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metricaly.Angular.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WidgetController : ControllerBase
    {
        private readonly IMediator mediator;

        public WidgetController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateWidgetCommand request)
        {
            return await mediator.Send(request);
        }

        [HttpGet("list/{applicationId}")]
        public async Task<ActionResult<List<WidgetDto>>> ListForApplication(Guid applicationId)
        {
            return await mediator.Send(new GetWidgetForApplicationQuery() { ApplicationId = applicationId });
        }

        [HttpGet("get/{widgetId}")]
        public async Task<ActionResult<WidgetDto>> GetWidget(Guid widgetId)
        {
            return await mediator.Send(new GetWidgetDetails() { WidgetId = widgetId });
        }
    }
}
