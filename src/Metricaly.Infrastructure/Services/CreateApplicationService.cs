﻿using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Services
{
    public class CreateApplicationService : ICreateApplicationService
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IApiKeyGenerator apiKeyGenerator;

        public CreateApplicationService(ApplicationDbContext applicationDbContext, IApiKeyGenerator apiKeyGenerator)
        {
            this.applicationDbContext = applicationDbContext;
            this.apiKeyGenerator = apiKeyGenerator;
        }

        public async Task<Application> CreateAsync(string applicationName, string userId)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
                throw new ArgumentNullException(nameof(applicationName));

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            Application application = new Application();
            application.CreatedDate = DateTime.UtcNow;
            application.Name = applicationName;
            application.UserId = userId;

            application.ApiKey = await GenerateApiKeyAsync();

            await applicationDbContext.Applications.AddAsync(application);
            await applicationDbContext.SaveChangesAsync();

            return application;
        }

        private async Task<string> GenerateApiKeyAsync()
        {
            string apiKey;

            do
            {
                apiKey = apiKeyGenerator.Generate();
            } while (string.IsNullOrEmpty(apiKey) || !await IsApiKeyUniqueAsync(apiKey));

            return apiKey;
        }

        private async Task<bool> IsApiKeyUniqueAsync(string apiKey)
        {
            return await applicationDbContext.Applications
                .FirstOrDefaultAsync(a => a.ApiKey.Equals(apiKey)) == null;
        }
    }
}
