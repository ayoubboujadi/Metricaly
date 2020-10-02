using Metricaly.Core.Entities;
using Metricaly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Interfaces
{
    public interface IApplicationRepository
    {
        Task<Application> GetByApiKeyAsync(string apiKey);
        Task<Application> GetByIdAndUserAsync(long id, string userId);
    }

    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ApplicationDbContext applicationDbContext;

        public ApplicationRepository(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<Application> GetByApiKeyAsync(string apiKey)
        {
            var application = await applicationDbContext.Applications
                        .FirstOrDefaultAsync(a => a.ApiKey.Equals(apiKey));

            return application;
        }

        public async Task<Application> GetByIdAndUserAsync(long id, string userId)
        {
            var application = await applicationDbContext.Applications.
                FirstOrDefaultAsync(x => x.Id == id && x.UserId.Equals(userId));

            return application;
        }
    }
}
