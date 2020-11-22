using Metricaly.Core.Entities;
using Metricaly.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metricaly.PublicApi.Services
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

        public async Task<Metric> FindAsync(Guid applicationId, string @namespace, string name)
        {
            string key = GetMetricCacheKey(applicationId, @namespace, name);

            var cachedMetric = cache.Get<Metric>(key);

            if (cachedMetric != null)
                return cachedMetric;


            var dbMetric = await metricRepository.FindAsync(applicationId, @namespace, name);

            if (dbMetric != null)
            {
                AddMetricToCache(dbMetric);
            }

            return dbMetric;
        }

        public async Task<List<Metric>> FindByApplicationAsync(Guid applicationId)
        {
            return await metricRepository.FindByApplicationAsync(applicationId);
        }

        public async Task<Guid> InsertAsync(Metric metric)
        {
            string key = GetMetricCacheKey(metric.ApplicationId, metric.Namespace, metric.Name);

            var metricId = await metricRepository.InsertAsync(metric);

            AddMetricToCache(metric);

            return metricId;
        }

        private string GetMetricCacheKey(Guid applicationId, string @namespace, string name)
        {
            return $"{applicationId}:{@namespace}:{name}";
        }

        private void AddMetricToCache(Metric metric)
        {
            if (metric == null) return;

            string key = GetMetricCacheKey(metric.ApplicationId, metric.Namespace, metric.Name);

            var options = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(5),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            cache.Set(key, metric, options);
        }
    }
}
