using FluentValidation;
using FluentValidation.Results;
using Metricaly.Core.Common.Utils;
using Metricaly.PublicApi.Requests;
using System;
using System.Collections.Generic;

namespace Metricaly.PublicApi.Validators
{
    public class CollectAggregatedMetricCollectionRequestValidator : AbstractValidator<List<CollectAggregatedMetricRequest>>
    {
        public CollectAggregatedMetricCollectionRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => x.Count >= 1 && x.Count <= 10)
                .WithMessage("Should provide between 1 and 10 Metrics.");

            RuleForEach(x => x).SetValidator(new CollectAggregatedMetricRequestValidator());
        }
    }
}
