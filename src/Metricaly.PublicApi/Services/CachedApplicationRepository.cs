using Metricaly.Core.Entities;
using Metricaly.Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Metricaly.PublicApi.Services
{
    public class CachedApplicationRepository : IApplicationRepository
    {
        private readonly ApplicationRepository applicationRepository;
        private readonly IMemoryCache cache;

        public CachedApplicationRepository(ApplicationRepository applicationRepository, IMemoryCache cache)
        {
            this.applicationRepository = applicationRepository;
            this.cache = cache;
        }

        public async Task<Application> GetByApiKeyAsync(string apiKey)
        {
            return await cache.GetOrCreateAsync(apiKey, async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await applicationRepository.GetByApiKeyAsync(apiKey);
            });
        }

        public async Task<Application> GetByIdAndUserAsync(Guid id, string userId)
        {
            return await applicationRepository.GetByIdAndUserAsync(id, userId);
        }
    }
}
