using FluentValidation;
using FluentValidation.Results;
using Metricaly.Core.Common.Utils;
using Metricaly.PublicApi.Requests;
using System;

namespace Metricaly.PublicApi.Validators
{
    public class CollectAggregatedMetricRequestValidator : AbstractValidator<CollectAggregatedMetricRequest>
    {
        public CollectAggregatedMetricRequestValidator()
        {
            RuleFor(x => x.MetricName).MetricNameValidation();

            RuleFor(x => x.MetricNamespace).MetricNamespaceValidation();

            RuleFor(x => x.Timestamp).TimestampValidation();

            RuleFor(x => x.Max)
                .NotNull()
                .WithMessage("Max must be provided.");

            RuleFor(x => x.Min)
                .NotNull()
                .WithMessage("Min must be provided.");

            RuleFor(x => x.Sum)
                .NotNull()
                .WithMessage("Sum must be provided.");

            RuleFor(x => x.Average)
                .NotNull()
                .WithMessage("Average must be provided.");

            RuleFor(x => x.SamplesCount)
                .NotNull()
                .WithMessage("SamplesCount must be provided.");
        }
    }
}
