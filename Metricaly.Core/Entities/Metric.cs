using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Entities
{
    public class Metric : BaseEntity
    {
        public Metric(string name, string @namespace, long applicationId)
        {
            Name = name;
            Namespace = @namespace;
            ApplicationId = applicationId;
            CreatedDate = DateTime.UtcNow;
        }

        public string Name { get; set; }
        public string Namespace { get; set; }
        public DateTime CreatedDate { get; set; }
        public long ApplicationId { get; set; }
    }
}
