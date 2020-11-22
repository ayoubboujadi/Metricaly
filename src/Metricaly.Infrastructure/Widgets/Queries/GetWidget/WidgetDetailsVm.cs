using Metricaly.Core.Widgets;
using Metricaly.Infrastructure.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Metricaly.Infrastructure.Widgets.Queries.GetWidget
{
    public class WidgetDetailsVm<T> where T : IWidgetData 
    {
        public WidgetDto Widget { get; set; }
        public T WidgetData { get; set; }
    }
}
