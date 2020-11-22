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
        Task<Metric> FindAsync(Guid applicationId, string @namespace, string name);
        Task<Guid> InsertAsync(Metric metric);
        Task<List<Metric>> FindByApplicationAsync(Guid applicationId);
    }

    public class MetricRepository : IMetricRepository
    {
        private readonly ApplicationDbContext applicationDbContext;

        public MetricRepository(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<Metric> FindAsync(Guid applicationId, string @namespace, string name)
        {
            return await applicationDbContext.Metrics
                .FirstOrDefaultAsync(metric => metric.ApplicationId == applicationId && metric.Namespace.Equals(@namespace) && metric.Name.Equals(name));
        }

        public async Task<Metric> FindAsync(Guid applicationId, Guid metricId)
        {
            return await applicationDbContext.Metrics
                .FirstOrDefaultAsync(metric => metric.ApplicationId == applicationId && metric.Id == metricId);
        }

        public async Task<List<Metric>> FindByApplicationAsync(Guid applicationId)
        {
            return await applicationDbContext.Metrics
                .Where(metric => metric.ApplicationId == applicationId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Guid> InsertAsync(Metric metric)
        {
            await applicationDbContext.AddAsync(metric);
            await applicationDbContext.SaveChangesAsync();

            return metric.Id;
        }
    }
}
