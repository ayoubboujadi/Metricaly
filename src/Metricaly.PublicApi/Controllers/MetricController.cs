using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Metricaly.PublicApi.Controllers
{
    [Route("[controller]")]
    public class MetricController : ApplicationApiControllerBase
    {
        private readonly IMetricsCollectionService metricsCollectionService;
        private readonly IMetricRepository metricRepository;

        public MetricController(IMetricRepository metricRepository, IMetricsCollectionService metricsCollectionService)
        {
            this.metricRepository = metricRepository;
            this.metricsCollectionService = metricsCollectionService;
        }

        [HttpGet("send/{namespace}/{metricName}/{value}")]
        public async Task<ActionResult> Collect(string @namespace, string metricName, double value)
        {
            var metric = await metricRepository.FindAsync(Application.Id, @namespace, metricName);

            if (metric == null)
            {
                metric = new Metric(metricName, @namespace, Application.Id);
                await metricRepository.InsertAsync(metric);
            }

            await metricsCollectionService.CollectAsync(Application.Id, metricName, @namespace, value);

            return Ok(new { metricId = metric.Id });
        }

        [HttpGet("ping")]
        public ActionResult<string> Ping()
        {
            return "pong";
        }
    }
}
