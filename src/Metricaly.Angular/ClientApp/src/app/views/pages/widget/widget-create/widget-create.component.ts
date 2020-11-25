import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NbToastrService, NbGlobalPhysicalPosition } from '@nebular/theme';

import { ApplicationClient, WidgetClient, ApplicationDto, WidgetDto, CreateWidgetCommand } from '@app/web-api-client';

@Component({
  selector: 'app-widget-create',
  templateUrl: './widget-create.component.html',
  styleUrls: ['./widget-create.component.css']
})
export class WidgetCreateComponent implements OnInit {

  selectedApplicationId: string;
  applications: ApplicationDto[] = [];
  appsLoading = false;
  widgets: WidgetDto[];
  widgetsLoading = false;
  widgetName: string;
  creatingWidget = false;
  widgetType: string;
  defaultChoice = 'LineChart';

  constructor(private router: Router, private applicationDataService: ApplicationClient, private widgetService: WidgetClient,
    private toastrService: NbToastrService) { }

  ngOnInit(): void {
    this.loadApplications();
  }

  loadApplications() {
    this.appsLoading = true;
    this.applicationDataService.list()
      .subscribe(
        (result) => {
          this.appsLoading = false;
          this.applications = result;
        },
        (error) => {
          console.error(error);
        }
      );
  }

  loadApplicationWidgets() {
    this.widgetsLoading = true;
    this.widgetService.list(this.selectedApplicationId)
      .subscribe(
        (result) => {
          this.widgetsLoading = false;
          this.widgets = result;
        },
        (error) => {
          console.error(error);
        }
      );
  }

  createWidget() {
    const widget = CreateWidgetCommand.fromJS({
      applicationId: this.selectedApplicationId,
      name: this.widgetName,
      widgetType: 'LineChart'
    });


    this.widgetService.create(widget)
      .subscribe(
        (result) => {
          this.toastrService.show('Success', 'Widget was created successfully!',
          { position: NbGlobalPhysicalPosition.TOP_RIGHT, status: 'success' });
          this.router.navigate(['widget-builder/line-chart', result]);
        },
        (error) => {
          console.error(error);
        }
      );
  }

  choose(event) {
    console.log('type changed: ' + event);
  }
}
