using Metricaly.Core.Entities;
using Metricaly.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metricaly.Angular.Services
{
    public class CachedMetricRepository : IMetricRepository
    {
        private readonly MetricRepository metricRepository;
        private readonly IMemoryCache cache;

        public CachedMetricRepository(MetricRepository metricRepository, IMemoryCache cache)
        {
            this.metricRepository = metricRepository;
            this.cache = cache;
        }

        public async Task<Metric> Get(long applicationId, string @namespace, string name)
        {
            string key = GetMetricCacheKey(applicationId, @namespace, name);

            return await cache.GetOrCreateAsync<Metric>(key, async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(5);
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await metricRepository.Get(applicationId, @namespace, name);
            });
        }

        public async Task<List<Metric>> GetByApplication(long applicationId)
        {
            return await metricRepository.GetByApplication(applicationId);
        }

        public async Task<long> Insert(Metric metric)
        {
            string key = GetMetricCacheKey(metric.ApplicationId, metric.Namespace, metric.Name);

            var metricId = await metricRepository.Insert(metric);



            return metricId;
        }

        private string GetMetricCacheKey(long applicationId, string @namespace, string name)
        {
            return $"{applicationId}:{@namespace}:{name}";
        }

    }
}
