using AdysTech.InfluxDB.Client.Net;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Interfaces;
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
        private readonly IConnectionMultiplexer connectionMultiplexer;
        private readonly ILogger<AggregatorBackgroudService> logger;
        public AggregatorBackgroudService(IConnectionMultiplexer connectionMultiplexer, ILogger<AggregatorBackgroudService> logger)
        {
            this.connectionMultiplexer = connectionMultiplexer;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            InfluxDBClient client = new InfluxDBClient("http://localhost:8086/");
            var aggregatorService = new MetricsAggregator(connectionMultiplexer, client);

            var redisDb = connectionMultiplexer.GetDatabase();

            // Handle any backlog data
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    if (await DequeuAndAggregateAsync(redisDb, aggregatorService) == null)
            //    {
            //        break;
            //    }

            //    await Task.Delay(20, stoppingToken);
            //}

            while (true)
            {
                var key = await DequeuAndAggregateStreamAsync(redisDb, aggregatorService);

                if (key == null)
                    await Task.Delay(100);
            }

            //ISubscriber sub = connectionMultiplexer.GetSubscriber();
            //sub.Subscribe("metrics").OnMessage(async message =>
            //{
            //    await DequeuAndAggregateStreamAsync(redisDb, aggregatorService);
            //});

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(60 * 1000, stoppingToken);
            }
        }

        private async Task<string> DequeuAndAggregateStreamAsync(IDatabase redisDb, MetricsAggregator aggregatorService)
        {
            var entries = await redisDb.StreamReadGroupAsync("metrics_stream", "metrics_consumer_group", "aggregator_1", ">", count: 1);
            if (entries.Length <= 0)
                return null;

            var redisValue = entries[0].Values[0];

            //var redisValue = await redisDb.ListRightPopAsync("mqueue");
            if (!redisValue.Value.IsNull && !redisValue.Value.IsNullOrEmpty)
            {
                var key = redisValue.Value.ToString().Trim('"');
                Stopwatch stopwatch = Stopwatch.StartNew();
                int count = -1;
                try
                {
                    count = await aggregatorService.AggregateAsync(key);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Exception in AggregatorBackgroundService");
                }

                logger.LogInformation($"Aggregation for [{key}] took:" + stopwatch.ElapsedMilliseconds + "ms with " + count + " items.");
                return key;
            }
            else
                return null;
        }

        private async Task<string> DequeuAndAggregateListAsync(IDatabase redisDb, MetricsAggregator aggregatorService)
        {
            var redisValue = await redisDb.ListRightPopAsync("mqueue");
            if (!redisValue.IsNull && !redisValue.IsNullOrEmpty)
            {
                var key = redisValue.ToString().Trim('"');
                Stopwatch stopwatch = Stopwatch.StartNew();
                int count = -1;
                try
                {
                    count = await aggregatorService.AggregateAsync(key);
                }
                catch (Exception ex)
                { }
                logger.LogInformation($"Aggregation for [{key}] took:" + stopwatch.ElapsedMilliseconds + "ms with " + count + " items.");
                return key;
            }
            else
                return null;
        }

    }
}
