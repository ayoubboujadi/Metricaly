using MediatR;
using Metricaly.Core.Widgets.LineChartWidget;
using Metricaly.Core.Widgets.SimpleNumber;
using Metricaly.Infrastructure.Widgets.Commands.UpdateWidget;
using Metricaly.Infrastructure.Widgets.Queries.GetWidget;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Angular.Controllers.Widgets
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SimpleNumberWidgetController : ControllerBase
    {
        private readonly IMediator mediator;

        public SimpleNumberWidgetController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] UpdateWidgetCommand<SimpleNumberWidget> requestData)
        {
            await mediator.Send(requestData);
            return NoContent();
        }

        [HttpGet("get/{widgetId}")]
        public async Task<ActionResult<WidgetDetailsVm<SimpleNumberWidget>>> Get(Guid widgetId)
        {
            var result = await mediator.Send(new GetWidgetsQuery<SimpleNumberWidget>() { WidgetsIds = new List<Guid>() { widgetId } });
            return result.FirstOrDefault();
        }

        [HttpPost("get")]
        public async Task<ActionResult<List<WidgetDetailsVm<SimpleNumberWidget>>>> ReadMultiple([FromBody] List<Guid> widgetIds)
        {
            return await mediator.Send(new GetWidgetsQuery<SimpleNumberWidget>() { WidgetsIds = widgetIds });
        }
    }
}
