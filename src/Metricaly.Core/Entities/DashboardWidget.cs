using Metricaly.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Core.Entities
{
    public class DashboardWidget : BaseEntity
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Guid DashboardId { get; set; }
        public Dashboard Dashboard { get; set; }

        public Guid WidgetId { get; set; }
        public Widget Widget { get; set; }
    }
}
