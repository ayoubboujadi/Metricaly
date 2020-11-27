import { ReactiveFormsModule } from '@angular/forms';
import { Component, OnInit, Input, ViewChild, ComponentFactoryResolver } from '@angular/core';
import { DashboardGridWidget } from '../dashboard.component';
import { LineChartWidgetComponent } from '@app/views/widgets/line-chart-widget/line-chart-widget.component';
import { WidgetComponent } from './../../widgets/widget-component.interface';

@Component({
  selector: 'app-dashboard-item',
  templateUrl: './dashboard-item.component.html',
  styleUrls: ['./dashboard-item.component.css']
})
export class DashboardItemComponent implements OnInit {

  @ViewChild(WidgetComponent) widgetElement: WidgetComponent;

  @Input() item: DashboardGridWidget;
  @Input() selectedTimePeriod: any;
  @Input() applicationId: string;

  contextMenuItem: any;

  constructor(private componentFactoryResolver: ComponentFactoryResolver) { }

  ngOnInit(): void {
  }

  loadData() {
    this.widgetElement?.loadPlottedMetricsData();
  }

  reloadWidget() {
   this.widgetElement?.hardReloadPlottedMetrics();
  }

  getWidgetContextMenu() {
    if (this.contextMenuItem !== null && this.contextMenuItem !== undefined) {
      return this.contextMenuItem;
    }

    this.contextMenuItem = [{
      title: 'Remove from Dashboard',
      data: {
        widgetId: this.item.dashboardWidgetId,
        option: 'delete'
      }
    }, {
      title: 'Edit Widget',
      data: {
        widgetId: this.item.dashboardWidgetId,
        option: 'edit'
      }
    }
    ];
    return this.contextMenuItem;
  }

}
