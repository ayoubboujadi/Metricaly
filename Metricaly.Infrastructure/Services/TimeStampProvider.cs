using Metricaly.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Infrastructure.Services
{
    public class TimeStampProvider : ITimeStampProvider
    {
        public long GetTimeStamp()
        {
            return (long)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public long GetTimeStamp(double granularity)
        {
            var currentSeconds = GetTimeStamp();
            var timestamp = (long)(Math.Floor(currentSeconds / granularity) * granularity);
            return timestamp;
        }
    }
}
