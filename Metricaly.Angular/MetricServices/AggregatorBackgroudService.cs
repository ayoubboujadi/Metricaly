using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Web
{
    public class AggregatorBackgroudService : BackgroundService
    {
        private readonly IRedisCacheClient redisCacheClient;
        private readonly ILogger<AggregatorBackgroudService> logger;
        public AggregatorBackgroudService(IRedisCacheClient redisCacheClient, ILogger<AggregatorBackgroudService> logger)
        {
            this.redisCacheClient = redisCacheClient;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var aggregatorService = new MetricsAggregator(redisCacheClient);

            while (!stoppingToken.IsCancellationRequested)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                int count = -1;
                try
                {
                    count = await aggregatorService.Aggregate();
                }
                catch (Exception ex)
                {
                }

                stopwatch.Stop();

                logger.LogInformation("Aggregation took:" + stopwatch.ElapsedMilliseconds + "ms with " + count + " items.");

                await Task.Delay(5 * 1000, stoppingToken);
            }
        }
    }
}
