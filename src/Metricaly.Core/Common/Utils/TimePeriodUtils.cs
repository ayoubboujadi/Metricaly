using Metricaly.Core.Common.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Common.Utils
{
    public static class TimePeriodUtils
    {
        public static TimePeriod Parse(long? startTime, long? endTime, string liveTimeSpan)
        {
            if(!string.IsNullOrWhiteSpace(liveTimeSpan))
            {
                var timespan = LiveTimespan.GetTimespan(liveTimeSpan);
                var currentTimestamp = TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Seconds);
                return new TimePeriod()
                {
                    StartTimestamp = currentTimestamp - (int)timespan.TotalSeconds,
                    EndTimestamp = currentTimestamp
                };
            }

            if(startTime == null)
            {
                throw new Exception($"At least one of the following two parameters should be provided: {nameof(startTime)}, {nameof(endTime)}.");
            }

            long endTimestamp = endTime == null ? TimeStampProvider.GetCurrentTimeStamp(TimePrecisionUnit.Seconds) : (long)endTime;

            return new TimePeriod()
            {
                StartTimestamp = (long)startTime,
                EndTimestamp = endTimestamp
            };
        }
    }
}
