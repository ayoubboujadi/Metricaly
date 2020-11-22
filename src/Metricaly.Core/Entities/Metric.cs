using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Entities
{
    public class Metric : BaseEntity
    {
        public Metric()
        {
        }

        public Metric(string name, string @namespace, Guid applicationId)
        {
            Name = name;
            Namespace = @namespace;
            ApplicationId = applicationId;
        }

        public string Name { get; set; }
        public string Namespace { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
