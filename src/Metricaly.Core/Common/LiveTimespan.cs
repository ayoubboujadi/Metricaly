using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Common
{
    public class LiveTimespan
    {
        private Dictionary<string, TimeSpan> values;
        private static readonly LiveTimespan instance = new LiveTimespan();

        public static Dictionary<string, TimeSpan> Values => instance.values;

        static LiveTimespan()
        {
        }

        private LiveTimespan()
        {
            Init();
        }

        private void Init()
        {
            values = new Dictionary<string, TimeSpan>()
            {
                {"1m", TimeSpan.FromSeconds(60) },
                {"5m", TimeSpan.FromMinutes(5) },
                {"15m", TimeSpan.FromMinutes(15) },
                {"30m", TimeSpan.FromMinutes(30) },
                {"1h",TimeSpan.FromHours(1) },
                {"3h", TimeSpan.FromHours(3) },
                {"6h", TimeSpan.FromHours(6) },
                {"12h", TimeSpan.FromHours(12) },
                {"1d", TimeSpan.FromHours(24) },
            };
        }

        public static TimeSpan GetTimespan(string value)
        {
            if (!instance.values.ContainsKey(value))
            {
                throw new Exception($"LiveTimespan value does not exist: {value}. Possible values are: {string.Join(", ", instance.values.Keys)}.");
            }

            return instance.values[value];
        }
    }
}
