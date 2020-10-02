using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Metricaly.Angular.Filters;
using Metricaly.Core.Entities;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Identity;
using Metricaly.Infrastructure.Interfaces;
using Metricaly.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Metricaly.Web.Controllers
{

    [Route("[controller]")]
    public class MetricController : ControllerBase
    {
        private readonly ILogger<MetricController> logger;
        private readonly IRedisCacheClient redisCacheClient;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMetricRepository metricRepository;
        private readonly IApplicationRepository applicationRepository;

        public MetricController(ILogger<MetricController> logger, IRedisCacheClient redisCacheClient, UserManager<ApplicationUser> userManager, IMetricRepository metricRepository, IApplicationRepository applicationRepository)
        {
            this.logger = logger;
            this.redisCacheClient = redisCacheClient;
            this.userManager = userManager;
            this.metricRepository = metricRepository;
            this.applicationRepository = applicationRepository;
        }

        [HttpGet("get/{applicationId}/{namespace}/{metricName}/{startTimestamp}/{endTimestamp}")]
        public async Task<ActionResult<MetricValue[]>> GetMetricValues(long applicationId, string @namespace, string metricName, long startTimestamp, long endTimestamp)
        {
            // TODO: data needs to be sampled down to one minute before we return it
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var application = await applicationRepository.GetByIdAndUserAsync(applicationId, userId);

            if (application == null)
                return NotFound(applicationId);

            var metric = await metricRepository.Get(applicationId, @namespace, metricName);

            if (metric != null)
            {
                MetricsRetriever metricsRetriever = new MetricsRetriever(redisCacheClient);
                var metrics = await metricsRetriever.GetMetricValues(metric, new TimePeriod() { StartTimestamp = startTimestamp, EndTimestamp = endTimestamp });
                var sampledMetrics = new List<MetricValue>();

                int samplingValue = 5 * 60;
                long timestamp = (long)(Math.Floor(metrics[0].TimeStamp / (double)samplingValue) * samplingValue);

                // If less than 4 hours                         => 10 seconds sampling
                // Between 4 hours and 24 hours                 => 1 min sampling
                // Between 24 hours and a 24 hours and 5 days   => 5 min sampling
                // More than 5 days                             => 15 min sampling


                for (long i = timestamp; i < metrics.LastOrDefault().TimeStamp; i += samplingValue)
                {
                    var temp = metrics.Where(x => x.TimeStamp >= i && x.TimeStamp < i + samplingValue);
                    if (temp.Any())
                    {
                        long? value;
                        try
                        {
                            value = temp.Sum(x => x.Value);
                        }
                        catch (OverflowException)
                        {
                            value = long.MaxValue;
                        }

                        sampledMetrics.Add(new MetricValue() { TimeStamp = i, Value = value});
                    }
                    else
                    {
                       sampledMetrics.Add(new MetricValue() { TimeStamp = i, Value = null });
                    }
                }
                return sampledMetrics.ToArray();
            }

            return NotFound();
        }

        [HttpGet("{applicationId}/metrics")]
        public async Task<ActionResult<List<Metric>>> ListMetrics(long applicationId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var application = await applicationRepository.GetByIdAndUserAsync(applicationId, userId);

            if (application == null)
                return NotFound(applicationId);

            var metrics = await metricRepository.GetByApplication(application.Id);

            return metrics;
        }
    }
}
