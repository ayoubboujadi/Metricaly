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
            Widgets = new List<Widget>();
            Dashboards = new List<Dashboard>();
        }

        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string UserId { get; set; }
        public IList<Metric> Metrics { get; private set; }
        public IList<Widget> Widgets { get; private set; }
        public IList<Dashboard> Dashboards { get; private set; }
    }
}
