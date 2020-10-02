using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Interfaces
{
    public interface ITimeStampProvider
    {
        long GetTimeStamp();
        long GetTimeStamp(double granularity);
    }
}
