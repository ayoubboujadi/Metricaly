using AdysTech.InfluxDB.Client.Net;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Common.Extensions;
using Metricaly.Infrastructure.Services;
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
    public class QueueBackgroudService : BackgroundService
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;
        private readonly ILogger<QueueBackgroudService> logger;
        public QueueBackgroudService(IConnectionMultiplexer connectionMultiplexer, ILogger<QueueBackgroudService> logger)
        {
            this.connectionMultiplexer = connectionMultiplexer;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var redisDb = connectionMultiplexer.GetDatabase();
            ISubscriber sub = connectionMultiplexer.GetSubscriber();
            try
            {
                redisDb.StreamCreateConsumerGroup("metrics_stream", "metrics_consumer_group", "$");
            }
            catch (Exception)
            { }

            while (!stoppingToken.IsCancellationRequested)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                Stopwatch stopwatchRead = null;
                Stopwatch stopwatchStreamAdding = null;
                Stopwatch stopwatchPublish = null;
                Stopwatch stopwatchRemove = null;

                int takeCount = 100;
                int count = -1;
                try
                {
                    var currentSeconds = TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Seconds, 10);
                    stopwatchRead = Stopwatch.StartNew();
                    var sortedSetEntries = await redisDb.SortedSetRangeByScoreWithScoresAsync("recentlyset", double.NegativeInfinity, currentSeconds, take: takeCount);
                    stopwatchRead.Stop();

                    var itemsToAdd = sortedSetEntries.Select(x => x.Element).ToArray();
                    count = itemsToAdd.Length;

                    if (itemsToAdd.Length > 0)
                    {
                        stopwatchStreamAdding = Stopwatch.StartNew();
                        foreach (var item in itemsToAdd)
                            await redisDb.StreamAddAsync("metrics_stream", "key", item);
                        stopwatchStreamAdding.Stop();

                        //stopwatchPublish = Stopwatch.StartNew();
                        //foreach (var item in itemsToAdd)
                        //{
                        //    await sub.PublishAsync("metrics", item);
                        //}
                        //stopwatchPublish.Stop();

                        stopwatchRemove = Stopwatch.StartNew();
                        await redisDb.SortedSetRemoveRangeByRankAsync("recentlyset", 0, itemsToAdd.Length - 1);
                        stopwatchRemove.Stop();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Exception in QueueBackgroundService");
                }

                stopwatch.Stop();
                if (count != 0)
                    logger.LogInformation("Adding to Queue took:" + stopwatch.ElapsedMilliseconds + "ms with " + count
                        + $" items | Read: {stopwatchRead?.ElapsedMilliseconds}, Stream: {stopwatchStreamAdding?.ElapsedMilliseconds}, "
                        + $" Publish: {stopwatchPublish?.ElapsedMilliseconds}, Remove: {stopwatchRemove?.ElapsedMilliseconds}.");

                int waitTime = 100;
                if (count >= takeCount)
                    waitTime = 0;
                else if (count == 0)
                    waitTime = 200;

                if (waitTime > 0)
                    await Task.Delay(waitTime, stoppingToken);
            }
        }
    }
}
