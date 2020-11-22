import { Component, OnInit } from '@angular/core';
import { NbDialogService } from '@nebular/theme';
import { DashboardDto, ApplicationDto, DashboardClient, ApplicationClient, FavoriteDashboardCommand } from '@app/web-api-client';
import { ConfirmDialogComponent } from '@app/views/shared/dialogs/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-dashboards-manage',
  templateUrl: './dashboards-manage.component.html',
  styleUrls: ['./dashboards-manage.component.css']
})
export class DashboardsManageComponent implements OnInit {

  appsLoading = true;
  applications: ApplicationDto[] = [];
  selectedApplicationId: string;
  dashboardsLoading = true;
  dashboards: DashboardDto[];

  constructor(private applicationClient: ApplicationClient, private dashboardClient: DashboardClient,
    private dialogService: NbDialogService) {
    this.loadApplications();
  }

  ngOnInit(): void {
  }

  loadApplications() {
    this.appsLoading = true;
    this.applicationClient.list()
      .subscribe(
        (result) => {
          this.appsLoading = false;
          this.applications = result;
          if (this.applications.length > 0) {
            this.handleSelectedApplicationChange(this.applications[0].id);
          }
        },
        (error) => {
          console.error(error);
        }
      );
  }

  loadDashboards() {
    this.dashboardsLoading = true;
    this.dashboardClient.list(this.selectedApplicationId)
      .subscribe(
        (result) => {
          this.dashboardsLoading = false;
          this.dashboards = result;
        },
        (error) => {
          console.error(error);
        }
      );
  }

  handleSelectedApplicationChange(newValue: string): void {
    if (newValue !== this.selectedApplicationId) {
      this.selectedApplicationId = newValue;
      this.loadDashboards();
    }
  }

  deleteDashboard(dashboard: DashboardDto): void {
    this.dialogService.open(ConfirmDialogComponent, {
      autoFocus: false,
      closeOnEsc: true,
      context: {
        title: 'Warning!',
        message: 'Are you sure you want to delete the dashboard "' + dashboard.name + '" ?',
      },
    }).onClose.subscribe(data => {
      if (data === true) {
        this.dashboardsLoading = true;
        this.dashboardClient.delete(dashboard.id)
          .subscribe(
            (result) => {
              this.loadDashboards();
            },
            (error) => {
              this.dashboardsLoading = false;
              console.error(error);
            }
          );
      }
    });
  }

  dashboardFavorite(dashboard: DashboardDto) {
    dashboard.isFavorite = !dashboard.isFavorite;
    this.dashboardClient.addFavorite(FavoriteDashboardCommand.fromJS({ dashboardId: dashboard.id, isFavorite: dashboard.isFavorite }))
      .subscribe(
        (result) => {
        },
        (error) => {
          console.error(error);
        }
      );
  }
}
