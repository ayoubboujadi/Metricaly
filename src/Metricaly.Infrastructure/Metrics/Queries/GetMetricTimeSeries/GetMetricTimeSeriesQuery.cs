using MediatR;
using Metricaly.Core.Common;
using Metricaly.Core.Common.Utils;
using Metricaly.Core.Entities;
using Metricaly.Core.Interfaces;
using Metricaly.Infrastructure.Data;
using Metricaly.Infrastructure.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Metricaly.Infrastructure.Metrics.Queries.GetMetricTimeSeries
{
    public class GetMetricTimeSeriesQuery : IRequest<MetricsTimeSeriesResultDto>
    {
        public long? StartTimestamp { get; set; }
        public long? EndTimestamp { get; set; }
        public string LiveSpan { get; set; }
        public int SamplingTime { get; set; }
        public Guid ApplicationId { get; set; }
        public List<MetricNamespaceDTO> Metrics { get; set; }
    }

    public class MetricNamespaceDTO
    {
        public string Guid { get; set; }
        public string MetricName { get; set; }
        public string Namespace { get; set; }
        public SamplingType SamplingType { get; set; } = SamplingType.Average;
    }

    public class GetMetricTimeSeriesQueryHandler : IRequestHandler<GetMetricTimeSeriesQuery, MetricsTimeSeriesResultDto>
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentUserService currentUserService;
        private readonly IMetricDownSampler metricDownSampler;
        private readonly IMetricsRetriever metricsRetriever;

        public GetMetricTimeSeriesQueryHandler(ApplicationDbContext context, ICurrentUserService currentUserService,
            IMetricDownSampler metricDownSampler, IMetricsRetriever metricsRetriever)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.metricDownSampler = metricDownSampler;
            this.metricsRetriever = metricsRetriever;
        }

        public async Task<MetricsTimeSeriesResultDto> Handle(GetMetricTimeSeriesQuery request, CancellationToken cancellationToken)
        {
            var dbMetrics = await ValidateMetrics(request);

            if (dbMetrics.Count == 0)
                return new MetricsTimeSeriesResultDto();

            var timePeriod = TimePeriodUtils.Parse(request.StartTimestamp, request.EndTimestamp, request.LiveSpan);

            MetricsTimeSeriesResultDto result = new MetricsTimeSeriesResultDto();

            for (int i = 0; i < request.Metrics.Count; i++)
            {
                var requestMetric = request.Metrics[i];
                var dbMetric = dbMetrics.FirstOrDefault(x => x.Name.Equals(requestMetric.MetricName) && x.Namespace.Equals(requestMetric.Namespace));

                if (dbMetric == null)
                    continue;

                // Retrieve the metrics
                var metricValues = await metricsRetriever.QueryAsync(dbMetric, timePeriod);

                // Down sample the metrics
                var samplingResult = metricDownSampler.DownSample(metricValues, request.SamplingTime, timePeriod.StartTimestamp, timePeriod.EndTimestamp);

                var metricValue = GetMetricTimeSeriesValueForSamplingType(requestMetric, samplingResult);

                result.Values.Add(metricValue);

                if (i == 0)
                {
                    result.Timestamps = samplingResult.MetricValues.Select(x => x.TimeStamp).ToList();
                    result.Count = samplingResult.MetricValues.Count;
                    result.SamplingValue = samplingResult.DownsamplingValue;
                }
            }

            return result;
        }

        private async Task<List<Metric>> ValidateMetrics(GetMetricTimeSeriesQuery request)
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

        private MetricTimeSeriesValueDto GetMetricTimeSeriesValueForSamplingType(MetricNamespaceDTO requestMetric, MetricDownsaplingResult samplingResult)
        {
            var metricValue = new MetricTimeSeriesValueDto()
            {
                MetricName = requestMetric.MetricName,
                Namespace = requestMetric.Namespace,
                Guid = requestMetric.Guid
            };

            switch (requestMetric.SamplingType)
            {
                case SamplingType.Average:
                    metricValue.Values = samplingResult.MetricValues.Select(x => x.Sum / x.Count).ToList();
                    break;
                case SamplingType.Max:
                    metricValue.Values = samplingResult.MetricValues.Select(x => x.Max).ToList();
                    break;
                case SamplingType.Min:
                    metricValue.Values = samplingResult.MetricValues.Select(x => x.Min).ToList();
                    break;
                case SamplingType.Sum:
                    metricValue.Values = samplingResult.MetricValues.Select(x => x.Sum).ToList();
                    break;
                case SamplingType.SamplesCount:
                    metricValue.Values = samplingResult.MetricValues.Select(x => x.Count).ToList();
                    break;
            }

            return metricValue;
        }
    }
}
