using System;

namespace Metricaly.Infrastructure.Common.Extensions
{
    public enum TimePrecisionUnit
    {
        Minutes,
        Seconds,
        Milliseconds,
        Microseconds,
        Nanoseconds
    }

    public static class TimeStampProvider
    {
        public static long GetCurrentTimeStamp(TimePrecisionUnit timePrecisionUnit)
        {
            var unixStart = new DateTime(1970, 1, 1);
            long ticks = DateTime.UtcNow.Ticks - unixStart.Ticks;

            return timePrecisionUnit switch
            {
                TimePrecisionUnit.Nanoseconds => ticks * 100,
                TimePrecisionUnit.Microseconds => ticks / 10,
                TimePrecisionUnit.Milliseconds => ticks / TimeSpan.TicksPerMillisecond,
                TimePrecisionUnit.Seconds => ticks / TimeSpan.TicksPerSecond,
                TimePrecisionUnit.Minutes => ticks / TimeSpan.TicksPerMinute,
                _ => throw new Exception($"TimePrecisionUnit not recognised."),
            };
        }

        public static long GetCurrentTimeStamp(TimePrecisionUnit timePrecisionUnit, double granularity)
        {
            var currentTimestamp = GetCurrentTimeStamp(timePrecisionUnit);
            var timestamp = (long)(Math.Floor(currentTimestamp / granularity) * granularity);
            return timestamp;
        }
    }
}
