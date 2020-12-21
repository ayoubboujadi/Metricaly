using Metricaly.Core.Common;
using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Interfaces;
using Metricaly.PublicApi.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metricaly.PublicApi.Controllers
{
    [Route("collect")]
    public class MetricCollectionController : ApplicationApiControllerBase
    {
        private readonly IMetricsCollectionService metricsCollectionService;
        private readonly IMetricRepository metricRepository;

        public MetricCollectionController(IMetricRepository metricRepository, IMetricsCollectionService metricsCollectionService)
        {
            this.metricRepository = metricRepository;
            this.metricsCollectionService = metricsCollectionService;
        }

        [HttpGet("single/{namespace}/{metricName}/{value}")]
        public async Task<ActionResult> Collect(string @namespace, string metricName, double value)
        {
            await CreateMetricIfItDoesntExist(metricName, @namespace);

            await metricsCollectionService.CollectSingleMetricAsync(Application.Id, metricName, @namespace, value);

            return Ok(new { status = "success" });
        }

        [HttpPost("multiple")]
        public async Task<ActionResult> Collect([FromBody] List<CollectSingleMetricRequest> request)
        {
            // TODO: First check if there are any duplicated metrics and aggregate them together
            foreach (var metricToCollect in request)
            {
                await CreateMetricIfItDoesntExist(metricToCollect.MetricNamespace, metricToCollect.MetricName);

                await metricsCollectionService.CollectSingleMetricAsync(Application.Id, metricToCollect.MetricName, metricToCollect.MetricNamespace,
                    metricToCollect.Value, metricToCollect.Timestamp);
            }

            return Ok(new { status = "success" });
        }

        [HttpPost("aggregated")]
        public async Task<ActionResult> CollectAggregated([FromBody] List<CollectAggregatedMetricRequest> metricsToCollect)
        {
            foreach (var metricToCollect in metricsToCollect)
            {
                await CreateMetricIfItDoesntExist(metricToCollect.MetricNamespace, metricToCollect.MetricName);

                var metricValue = new MetricValue
                {
                    Count = metricToCollect.SamplesCount,
                    Max = metricToCollect.Max,
                    Min = metricToCollect.Min,
                    Sum = metricToCollect.Sum,
                    TimeStamp = metricToCollect.Timestamp ?? 0
                };

                await metricsCollectionService.CollectAggregatedMetricAsync(Application.Id, metricToCollect.MetricName, metricToCollect.MetricNamespace, metricValue);
            }

            return Ok(new { status = "success" });
        }

        [HttpGet("ping")]
        public ActionResult<string> Ping()
        {
            return "pong";
        }

        private async Task<Metric> CreateMetricIfItDoesntExist(string metrinName, string metricNamespace)
        {
            var dbMetric = await metricRepository.FindAsync(Application.Id, metricNamespace, metrinName);

            if (dbMetric == null)
            {
                dbMetric = new Metric(metrinName, metricNamespace, Application.Id);
                await metricRepository.InsertAsync(dbMetric);
            }

            return dbMetric;
        }
    }
}
