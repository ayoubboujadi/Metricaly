using FluentValidation;
using Metricaly.Core.Common.Utils;
using System;

namespace Metricaly.PublicApi.Validators
{
    public static class ValidationExtension
    {
        public static IRuleBuilderOptions<T, string> MetricNameValidation<T>(this IRuleBuilder<T, string> rule)
        {
            return rule.NotEmpty()
                 .MaximumLength(50)
                 .MinimumLength(3)
                 .Matches("^[0-9a-zA-Z-_]{3,50}$")
                 .WithMessage("MetricName should have between 3 and 50 alphanumeric characters including '-' and '_'.");
        }

        public static IRuleBuilderOptions<T, string> MetricNamespaceValidation<T>(this IRuleBuilder<T, string> rule)
        {
            return rule.NotEmpty()
                 .MaximumLength(50)
                 .MinimumLength(3)
                 .Matches("^[0-9a-zA-Z-_]{3,50}$")
                 .WithMessage("MetricName should have between 3 and 50 alphanumeric characters including '-' and '_'.");
        }

        public static IRuleBuilderOptions<T, long?> TimestampValidation<T>(this IRuleBuilder<T, long?> rule)
        {
            bool BeWithinRange(long? timestamp)
            {
                var currentTimestamp = TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Seconds);
                var allowedRangeInSeconds = TimeSpan.FromHours(6).TotalSeconds;

                if (timestamp <= currentTimestamp - allowedRangeInSeconds || timestamp >= currentTimestamp + allowedRangeInSeconds)
                    return false;
                return true;
            }

            return rule.Must(BeWithinRange)
                .When(x => x != null)
                .WithMessage("Timestamp should be unix timestamp in seconds that falls between the last and next six hours.");
        }
    }
}
