using Metricaly.Infrastructure.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Angular.MetricServices
{
    public class ConsumersManagerWorker : BackgroundService
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;
        private readonly ILogger<AggregatorConsumerWorker> logger;

        private const int NumberOfConsumers = 3;
        private const string ConsumerNamePrefix = "consumer_";

        private const string AvailableConsumersListKey = "consumers.available"; // list
        private const string TakenConsumersListKey = "consumers.taken"; // list
        private const string StatusConsumersSortedSetKey = "consumers.status"; // sorted set

        public ConsumersManagerWorker(IConnectionMultiplexer connectionMultiplexer, ILogger<AggregatorConsumerWorker> logger)
        {
            this.connectionMultiplexer = connectionMultiplexer;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var redisDb = connectionMultiplexer.GetDatabase();
            while (!stoppingToken.IsCancellationRequested)
            {
                // consumers.available : list
                // consumers.taken : list
                // consumers.status : sorted set

                // Consumers side
                // 1. When a consumer runs, it should pick a name from the consumers.available list
                // 2. It should insert a value to the sorted set consumers.status

                // Manager side
                //  Case of first run:
                //      1. Create all the needed lists
                //

                var consumersNames = new List<string>();
                for (int i = 1; i <= NumberOfConsumers; i++)
                {
                    consumersNames.Add(ConsumerNamePrefix + i);
                }


                var oldAvailableConsumers = await redisDb.ListRangeAsync(AvailableConsumersListKey);
                var oldTakenConsumers = await redisDb.ListRangeAsync(AvailableConsumersListKey);

                // Prepare the available.consumers and the consumers.roundrobin lists
                foreach (var consumer in consumersNames)
                {
                    await redisDb.ListLeftPushAsync(AvailableConsumersListKey, consumer);
                }

                // Create the consumers.available list and add the consumers to it
                foreach (var consumer in consumersNames)
                {
                    await redisDb.SortedSetAddAsync("consumers.sortedset", consumer, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                }
            }
        }
    }
}
