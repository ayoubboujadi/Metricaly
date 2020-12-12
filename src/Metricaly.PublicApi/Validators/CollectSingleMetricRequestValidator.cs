using FluentValidation;
using FluentValidation.Results;
using Metricaly.Core.Common.Utils;
using Metricaly.PublicApi.Requests;
using System;

namespace Metricaly.PublicApi.Validators
{
    public class CollectSingleMetricRequestValidator : AbstractValidator<CollectSingleMetricRequest>
    {
        public CollectSingleMetricRequestValidator()
        {
            RuleFor(x => x.MetricName).MetricNameValidation();

            RuleFor(x => x.MetricNamespace).MetricNamespaceValidation();

            RuleFor(x => x.Timestamp).TimestampValidation();

            RuleFor(x => x.Value)
                .NotNull()
                .WithMessage("Value must be provided.");
        }
    }
}
