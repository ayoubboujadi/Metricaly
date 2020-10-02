using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Entities
{
    public class Application : BaseEntity
    {
        public Application()
        {
            Metrics = new List<Metric>();
        }

        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<Metric> Metrics { get; private set; }
    }
}
