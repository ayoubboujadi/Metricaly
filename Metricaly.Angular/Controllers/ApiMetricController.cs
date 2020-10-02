using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Metricaly.Angular.Filters;
using Metricaly.Core.Entities;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Interfaces;
using Metricaly.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace Metricaly.Web.Controllers
{
    [ApiController]
    [ApiKeyAuth]
    public class ApplicationApiController : ControllerBase
    {
        public Application Application => GetAuthenticatedApplicationObject();

        public ApplicationApiController()
        {
        }

        private Application GetAuthenticatedApplicationObject()
        {
            HttpContext.Items.TryGetValue(ApiKeyAuthAttribute.HttpContextItemsMiddlewareKey, out var application);
            return (Application)application;
        }
    }

    [Route("[controller]")]
    public class ApiMetricController : ApplicationApiController
    {
        private readonly ILogger<MetricController> logger;
        private readonly IRedisCacheClient redisCacheClient;
        private readonly IMetricRepository metricRepository;

        public ApiMetricController(ILogger<MetricController> logger, IRedisCacheClient redisCacheClient, IMetricRepository metricRepository)
        {
            this.logger = logger;
            this.redisCacheClient = redisCacheClient;
            this.metricRepository = metricRepository;
        }

        [HttpGet("add/{namespace}/{metricName}/{value}")]
        public async Task<string> AddMetric(string @namespace, string metricName, long value)
        {
            var metric = await metricRepository.Get(Application.Id, @namespace, metricName);

            if (metric == null)
            {
                metric = new Metric(metricName, @namespace, Application.Id);
                await metricRepository.Insert(metric);
            }

            // Store this metric value into the metric sorted set with the DateTime.Ticks as a key
            // Change the aggregator to 

            var incrementationService = new CounterService(redisCacheClient, new TimeStampProvider());
            var newCounterValue = await incrementationService.Increment(metric, value);

            return newCounterValue.ToString();
        }

        [HttpGet("get/{namespace}/{metricName}/{startTimestamp}/{endTimestamp}")]
        public async Task<ActionResult<MetricValue[]>> GetMetricValues(string @namespace, string metricName, long startTimestamp, long endTimestamp)
        {
            var metric = await metricRepository.Get(Application.Id, @namespace, metricName);

            if (metric != null)
            {
                MetricsRetriever metricsRetriever = new MetricsRetriever(redisCacheClient);

                return await metricsRetriever.GetMetricValues(metric, new TimePeriod() { StartTimestamp = startTimestamp, EndTimestamp = endTimestamp });
            }

            return NotFound();
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<Metric>>> ListMetrics()
        {
            var metrics = await metricRepository.GetByApplication(Application.Id);

            return metrics;
        }

        private static Random rand = new Random();
        [HttpGet("test")]
        public async Task<string> TestAddALot()
        {
            for (int i = 1599495262; i <= 1599495262 + 3600; i++)
            {
                var key = ((int)Math.Floor(i / 20.0)) * 20;

                int value = rand.Next(1, 200);

                var isAdded = await redisCacheClient.Db0.SortedSetAddIncrementAsync("alot2", key, value);
            }

            return "";
        }

        [HttpGet("test2")]
        public async Task<string> TestRound()
        {
            var value = "";
            for (int i = 0; i < 30; i++)
            {
                var currentSeconds = (int)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;

                var rounded = ((int)Math.Floor(currentSeconds / 15.0)) * 15;
                value += currentSeconds + " => " + rounded.ToString() + " </br>";

                await Task.Delay(999);
            }

            return value;
        }

        [HttpGet("getmetric/{name}")]
        public async Task<string> GetValue(string name)
        {
            var currentSeconds = (int)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;

            Stopwatch sw = Stopwatch.StartNew();
            var value = await redisCacheClient.Db0.SortedSetRangeByScoreAsync<string>(name, 0, currentSeconds);
            var took1 = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();
            var values = new List<int>();
            foreach (var item in value)
            {
                values.Add(int.Parse(item.Split(':')[1]));
            }
            var took2 = sw.ElapsedMilliseconds;

            StringBuilder sb = new StringBuilder();

            foreach (var item in value)
            {
                sb.Append(item + " , ");
            }

            return sb.ToString();
        }

    }
}
