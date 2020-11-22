using Metricaly.Infrastructure.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Infrastructure.Dashboards.Queries.GetDashboard
{
    public class DashboardDetailsVm
    {
        public DashboardDto Dashboard { get; set; }
        public IList<DashboardWidgetDto> DashboardWidgets { get; set; } = new List<DashboardWidgetDto>();
    }
}
