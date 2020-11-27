using MediatR;
using Metricaly.Core.Common;
using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Common.Extensions;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Dtos;
using Metricaly.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Metrics.Queries.GetMetricsAggregatedValue
{
    public class GetMetricsAggregatedValueQuery : IRequest<List<MetricAggregatedValueDto>>
    {
        public long StartTimestamp { get; set; }
        public long? EndTimestamp { get; set; }
        public Guid ApplicationId { get; set; }
        public List<AggregateMetricRequestDto> Metrics { get; set; }
    }

    public class AggregateMetricRequestDto
    {
        public string Guid { get; set; }
        public string MetricName { get; set; }
        public string Namespace { get; set; }
        public SamplingType SamplingType { get; set; } = SamplingType.Average;
    }

    public class GetMetricsAggregatedValueQueryHandler : IRequestHandler<GetMetricsAggregatedValueQuery, List<MetricAggregatedValueDto>>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMetricsRetriever metricsRetriever;

        public GetMetricsAggregatedValueQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService,
            IMetricsRetriever metricsRetriever)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.metricsRetriever = metricsRetriever;
        }

        public async Task<List<MetricAggregatedValueDto>> Handle(GetMetricsAggregatedValueQuery request, CancellationToken cancellationToken)
        {
            var dbMetrics = await ValidateMetrics(request);

            if (dbMetrics.Count == 0)
                return new List<MetricAggregatedValueDto>();

            long endTimestamp = request.EndTimestamp == null ? TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Seconds) : (long)request.EndTimestamp;

            var result = new List<MetricAggregatedValueDto>();

            for (int i = 0; i < request.Metrics.Count; i++)
            {
                var requestMetric = request.Metrics[i];
                var dbMetric = dbMetrics.FirstOrDefault(x => x.Name.Equals(requestMetric.MetricName) && x.Namespace.Equals(requestMetric.Namespace));

                if (dbMetric == null)
                    continue;

                // Retrieve the metrics
                var metricValues = await metricsRetriever.QueryAsync(dbMetric, new TimePeriod() { StartTimestamp = request.StartTimestamp, EndTimestamp = endTimestamp });

                var metricValue = GetMetricTimeSeriesValueForSamplingType(requestMetric, metricValues);

                result.Add(metricValue);
            }

            return result;
        }

        private async Task<List<Metric>> ValidateMetrics(GetMetricsAggregatedValueQuery request)
        {
            var currentUserId = currentUserService.GetCurrentUserId();
            try
            {
                //var dbMetrics = await (from application in context.Applications
                //                       join metric in context.Metrics
                //                       on application.Id equals metric.ApplicationId
                //                       where application.UserId == currentUserId && application.Id == request.ApplicationId
                //                           //&& request.Metrics.Any(x => x.MetricName == metric.Name && x.Namespace == metric.Namespace)
                //                       select metric
                //                        )
                //                        .AsNoTracking()
                //                        .ToListAsync();

                //var dbMetrics = await (from metric in context.Metrics
                //                       where metric.ApplicationId == request.ApplicationId
                //                       && request.Metrics.Any(x => x.MetricName == metric.Name && x.Namespace == metric.Namespace)
                //                       select metric
                //                        )
                //                        .AsNoTracking()
                //                        .ToListAsync();

                var dbMetrics = await context.Metrics.Where(metric => metric.ApplicationId == request.ApplicationId &&
                                    request.Metrics.Select(x => x.MetricName + x.Namespace).ToList().Contains(metric.Name + metric.Namespace))
                                    .AsNoTracking()
                                    .ToListAsync();
                 
                return dbMetrics;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private MetricAggregatedValueDto GetMetricTimeSeriesValueForSamplingType(AggregateMetricRequestDto requestMetric, MetricValue[] metricValues)
        {
            var metricValue = new MetricAggregatedValueDto()
            {
                MetricName = requestMetric.MetricName,
                Namespace = requestMetric.Namespace,
                Guid = requestMetric.Guid
            };

            switch (requestMetric.SamplingType)
            {
                case SamplingType.Average:
                    metricValue.Value = metricValues.Average(x => x.Sum / x.Count);
                    break;  
                case SamplingType.Max:
                    metricValue.Value  = metricValues.Max(x => x.Max);
                    break;
                case SamplingType.Min:
                    metricValue.Value = metricValues.Min(x => x.Min);
                    break;
                case SamplingType.Sum:
                    metricValue.Value = metricValues.Sum(x => x.Sum);
                    break;
                case SamplingType.SamplesCount:
                    metricValue.Value = metricValues.Sum(x => x.Count);
                    break;
            }

            return metricValue;
        }
    }
}
