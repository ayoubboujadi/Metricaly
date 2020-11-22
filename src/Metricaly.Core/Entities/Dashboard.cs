using Metricaly.Core.Widgets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Entities
{
    public class Dashboard : BaseEntity
    {
        public Dashboard()
        {

        }

        public Dashboard(string name, Guid applicationId)
        {
            Name = name;
            ApplicationId = applicationId;
        }

        public string Name { get; set; }
        public Guid ApplicationId { get; set; }
        public bool IsFavorite { get; set; }
        public List<DashboardWidget> DashboardWidgets { get; set; }
    }
}
