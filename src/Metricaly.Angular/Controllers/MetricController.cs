using MediatR;
using Metricaly.Infrastructure.Dtos;
using Metricaly.Infrastructure.Metrics.Queries.GetMetricsAggregatedValue;
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

        [HttpPost("timeseries")]
        public async Task<ActionResult<MetricsTimeSeriesResultDto>> GetTimeSeries([FromBody] GetMetricTimeSeriesQuery request)
        {
            return await mediator.Send(request);
        }

        [HttpPost("aggregatedvalue")]
        public async Task<ActionResult<List<MetricAggregatedValueDto>>> GetAggregatedValue([FromBody] GetMetricsAggregatedValueQuery request)
        {
            return await mediator.Send(request);
        }

        [HttpGet("{applicationId}/metrics")]
        public async Task<ActionResult<List<MetricDto>>> ListMetrics(Guid applicationId)
        {
            return await mediator.Send(new GetMetricsForApplicationQuery() { ApplicationId = applicationId });
        }
    }
}
