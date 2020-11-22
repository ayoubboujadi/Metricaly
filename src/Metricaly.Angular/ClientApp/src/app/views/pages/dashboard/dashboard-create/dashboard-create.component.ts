import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApplicationDto, CreateDashboardCommand, DashboardClient, ApplicationClient } from '@app/web-api-client';

@Component({
  selector: 'app-dashboard-create',
  templateUrl: './dashboard-create.component.html',
  styleUrls: ['./dashboard-create.component.css']
})
export class DashboardCreateComponent implements OnInit {
  selectedApplicationId: number;
  dashboardName: string;
  applications: ApplicationDto[] = [];
  appsLoading = false;
  savingDashboard = false;

  constructor(private applicationClient: ApplicationClient, private router: Router, private dashboardClient: DashboardClient) { }

  ngOnInit(): void {
    this.loadApplications();
  }

  loadApplications() {
    this.appsLoading = true;
    this.applicationClient.list()
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

  createdDashboard() {
    const dashboardCreateCommand = CreateDashboardCommand.fromJS({
      applicationId: this.selectedApplicationId,
      dashboardName: this.dashboardName
    });

    this.savingDashboard = true;
    this.dashboardClient.create(dashboardCreateCommand).subscribe((result) => {
      this.savingDashboard = false;
      this.router.navigate(['dashboard/view', result]);
    });
  }
}
