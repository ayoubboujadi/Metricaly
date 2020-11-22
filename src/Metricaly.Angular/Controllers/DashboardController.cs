using MediatR;
using Metricaly.Infrastructure.Dashboards.Commands.CreateDashboard;
using Metricaly.Infrastructure.Dashboards.Commands.DeleteDashboard;
using Metricaly.Infrastructure.Dashboards.Commands.FavoriteDashboard;
using Metricaly.Infrastructure.Dashboards.Commands.UpdateDashboard;
using Metricaly.Infrastructure.Dashboards.Queries.GetDashboard;
using Metricaly.Infrastructure.Dashboards.Queries.GetFavoriteDashboards;
using Metricaly.Infrastructure.Dashboards.Queries.ListDashboards;
using Metricaly.Infrastructure.Dtos;
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
    public class DashboardController : ControllerBase
    {
        private readonly IMediator mediator;

        public DashboardController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPut]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateDashboardCommand command)
        {
            return await mediator.Send(command);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] UpdateDashboardCommand command)
        {
            await mediator.Send(command);

            return NoContent();
        }

        [HttpGet("get/{dashboardId}")]
        public async Task<ActionResult<DashboardDetailsVm>> Get(Guid dashboardId)
        {
            return await mediator.Send(new GetDashboardQuery() { DashboardId = dashboardId });
        }

        [HttpGet("list/{applicationId}")]
        public async Task<ActionResult<List<DashboardDto>>> List(Guid applicationId)
        {
            try
            {
                return await mediator.Send(new ListDashboardsQuery() { ApplicationId = applicationId });
            }
            catch (Exception ex)
            {
            }

            return NoContent();
        }

        [HttpDelete("{dashboardId}")]
        public async Task<ActionResult> Delete(Guid dashboardId)
        {
            await mediator.Send(new DeleteDashboardCommand() { DashboardId = dashboardId });

            return NoContent();
        }

        [HttpGet("get/favorite")]
        public async Task<ActionResult<List<DashboardDto>>> GetFavoriteList()
        {
            return await mediator.Send(new GetFavoriteDashboardsQuery());
        }

        [HttpPost("add/favorite")]
        public async Task<ActionResult> AddFavorite([FromBody] FavoriteDashboardCommand command)
        {
            await mediator.Send(command);
            return NoContent();
        }
    }
}
