using Metricaly.Core.Widgets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Entities
{
    public class Widget : BaseEntity
    {
        public Widget()
        {
        }
        
        public Widget(string name, Guid applicationId)
        {
            Name = name;
            ApplicationId = applicationId;
        }

        public string Name { get; set; }
        public string Type { get; set; }

        public string Data { get; set; }

        public Guid ApplicationId { get; set; }
    }
}
