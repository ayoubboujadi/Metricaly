using FluentValidation;
using Metricaly.PublicApi.Requests;
using System.Collections.Generic;

namespace Metricaly.PublicApi.Validators
{
    public class CollectSingleMetricCollectionRequestValidator : AbstractValidator<List<CollectSingleMetricRequest>>
    {
        public CollectSingleMetricCollectionRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => x.Count >= 1 && x.Count <= 10)
                .WithMessage("Should provide between 1 and 10 Metrics.");

            RuleForEach(x => x).SetValidator(new CollectSingleMetricRequestValidator());
        }
    }
}
