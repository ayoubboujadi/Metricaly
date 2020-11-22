import { Component, OnInit, Input, ViewChild, TemplateRef, ComponentFactoryResolver, ComponentFactory, ComponentRef, ViewContainerRef } from '@angular/core';
import { DashboardGridWidget } from '../dashboard.component';

@Component({
  selector: 'app-dashboard-item',
  templateUrl: './dashboard-item.component.html',
  styleUrls: ['./dashboard-item.component.css']
})
export class DashboardItemComponent implements OnInit {

  @ViewChild('widgetHost', { read: ViewContainerRef }) private widgetHost: ViewContainerRef;

  @Input() item: DashboardGridWidget;
  selectedTimePeriod: any;
  constructor(private componentFactoryResolver: ComponentFactoryResolver) { }

  ngOnInit(): void {
  }

  createComponent<T>(componentFactory: ComponentFactory<T>): ComponentRef<T> {
    const viewContainerRef = this.widgetHost;
    viewContainerRef.clear();

    const componentRef = viewContainerRef.createComponent<T>(componentFactory);

    return componentRef;
  }

}
