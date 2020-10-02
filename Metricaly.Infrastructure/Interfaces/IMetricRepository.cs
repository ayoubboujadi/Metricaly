using Metricaly.Core.Entities;
using Metricaly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Interfaces
{
    public interface IMetricRepository
    {
        Task<Metric> Get(long applicationId, string @namespace, string name);
        Task<long> Insert(Metric metric);
        Task<List<Metric>> GetByApplication(long applicationId);
    }

    public class MetricRepository : IMetricRepository
    {
        private readonly ApplicationDbContext applicationDbContext;

        public MetricRepository(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<Metric> Get(long applicationId, string @namespace, string name)
        {
            return await applicationDbContext.Metrics
                .FirstOrDefaultAsync(metric => metric.ApplicationId == applicationId && metric.Namespace.Equals(@namespace) && metric.Name.Equals(name));
        }

        public async Task<List<Metric>> GetByApplication(long applicationId)
        {
            return await applicationDbContext.Metrics
                .Where(metric => metric.ApplicationId == applicationId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<long> Insert(Metric metric)
        {
            await applicationDbContext.AddAsync(metric);
            await applicationDbContext.SaveChangesAsync();

            return metric.Id;
        }
    }
}
