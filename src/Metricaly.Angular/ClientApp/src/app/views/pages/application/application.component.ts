import { Component, OnInit } from '@angular/core';
import { NbToastrService, NbGlobalPhysicalPosition, NbDialogService } from '@nebular/theme';
import { ApplicationCreateComponent } from './application-create/application-create.component';
import { ApplicationDto, CreateApplicationCommand, ApplicationClient } from '@app/web-api-client';

@Component({
  selector: 'app-application',
  templateUrl: './application.component.html',
  styleUrls: ['./application.component.css']
})
export class ApplicationComponent implements OnInit {

  applications: ApplicationDto[];
  appsLoading = false;
  loading = false;

  constructor(private toastrService: NbToastrService, private dialogService: NbDialogService,
    private applicationClient: ApplicationClient) { }

  ngOnInit(): void {
    this.loadApplications();
  }

  createApplication(applicationName: string) {
    this.loading = true;

    return this.applicationClient.create(CreateApplicationCommand.fromJS({ applicationName: applicationName }))
      .subscribe(
        result => {
          this.loading = false;
          this.loadApplications();
          this.toastrService.show('Success', 'Application was created successfully!',
            { position: NbGlobalPhysicalPosition.TOP_RIGHT, status: 'success' });
        },
        (error) => {
          console.error(error);
        }
      );
  }

  loadApplications() {
    this.appsLoading = true;
    this.applicationClient.list()
      .subscribe(
        data => {
          this.appsLoading = false;
          this.applications = data;
        },
        error => {
          console.error(error);
        }
      );
  }

  handleCreateApplicationClick() {
    this.dialogService.open(ApplicationCreateComponent)
      .onClose.subscribe(appName => appName && this.createApplication(appName));
  }

  handleDeleteApplicationEvent(application) {
    if (application != null && application !== undefined) {
      // TODO: Add application delete logic
    }
  }
}
