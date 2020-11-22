using MediatR;
using Metricaly.Infrastructure.Dtos;
using Metricaly.Infrastructure.Metrics.Queries.GetMetricsForApplication;
using Metricaly.Infrastructure.Metrics.Queries.GetMetricTimeSeries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metricaly.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class MetricController : ControllerBase
    {
        private readonly IMediator mediator;

        public MetricController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("query")]
        public async Task<ActionResult<MetricsTimeSeriesResultDto>> GetMetricValues([FromBody] GetMetricTimeSeriesQuery request)
        {
            try
            {
                return await mediator.Send(request);
            }
            catch (Exception ex)
            { }
            return NoContent();
        }

        [HttpGet("{applicationId}/metrics")]
        public async Task<ActionResult<List<MetricDto>>> ListMetrics(Guid applicationId)
        {
            return await mediator.Send(new GetMetricsForApplicationQuery() { ApplicationId = applicationId });
        }
    }
}
