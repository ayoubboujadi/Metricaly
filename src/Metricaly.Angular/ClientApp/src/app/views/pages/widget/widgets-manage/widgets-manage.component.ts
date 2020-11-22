import { Component, OnInit } from '@angular/core';
import { NbDialogService } from '@nebular/theme';
import { ConfirmDialogComponent } from '@app/views/shared/dialogs/confirm-dialog/confirm-dialog.component';
import { ApplicationClient, ApplicationDto, WidgetClient, WidgetDto } from '@app/web-api-client';
import { ActivatedRoute } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-widgets-manage',
  templateUrl: './widgets-manage.component.html',
  styleUrls: ['./widgets-manage.component.css']
})
export class WidgetsManageComponent implements OnInit {

  appsLoading = true;
  applications: ApplicationDto[] = [];
  selectedApplicationId: string;
  widgetsLoading = true;
  widgets: WidgetDto[];

  constructor(private applicationClient: ApplicationClient, private widgetClient: WidgetClient,
    private dialogService: NbDialogService, private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      console.log(params);
      this.selectedApplicationId = params.application;
      this.loadApplications();
    }
    );
  }

  loadApplications() {
    this.appsLoading = true;
    this.applicationClient.list()
      .subscribe(
        (data) => {
          this.appsLoading = false;
          this.applications = data;
          if (this.selectedApplicationId !== undefined && this.selectedApplicationId !== null) {
            this.loadWidgets();
          } else if (this.applications.length > 0) {
            this.handleSelectedApplicationChange(this.applications[0].id);
          }
        },
        (error) => {
          console.error(error);
        }
      );
  }

  loadWidgets() {
    this.widgetsLoading = true;
    this.widgetClient.list(this.selectedApplicationId)
      .subscribe(
        (data) => {
          this.widgetsLoading = false;
          this.widgets = data;
        },
        (error) => {
          console.error(error);
        }
      );
  }

  handleSelectedApplicationChange(newSelectedApplicationId: string): void {
    if (newSelectedApplicationId !== this.selectedApplicationId) {
      this.selectedApplicationId = newSelectedApplicationId;
      this.loadWidgets();
    }
  }

  deleteWidget(widget: WidgetDto): void {
    this.dialogService.open(ConfirmDialogComponent, {
      autoFocus: false,
      closeOnEsc: true,
      context: {
        title: 'Warning!',
        message: 'Are you sure you want to delete the widget "' + widget.name + '" ?',
      },
    }).onClose.subscribe(shouldDelete => {
      if (shouldDelete) {
        this.widgetsLoading = true;
        //this.widgetClient.delete(widget.id);
        //.subscribe(
        //  (data) => {
        //    this.loadWidgets()
        //  },
        //  (error) => {
        //    this.widgetsLoading = false;
        //    console.error(error)
        //  }
        //);
      }
    });
  }

}
