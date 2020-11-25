import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NbToastrService, NbGlobalPhysicalPosition, NbDialogService } from '@nebular/theme';
import { ConfirmDialogComponent } from '@app/views/shared/dialogs/confirm-dialog/confirm-dialog.component';
import { ApplicationDto } from '@app/web-api-client';

@Component({
  selector: 'app-application-manage',
  templateUrl: './application-manage.component.html',
  styleUrls: ['./application-manage.component.css']
})
export class ApplicationManageComponent implements OnInit {

  @Input() applications: ApplicationDto[];

  @Output() deleteApplicationEmitter: EventEmitter<ApplicationDto> = new EventEmitter();

  constructor(private toastrService: NbToastrService, private dialogService: NbDialogService) { }

  ngOnInit(): void {
  }

  handleApiKeyCopied(text) {
    this.toastrService.show('', 'ApiKey copied to clipboard!', {
      position: NbGlobalPhysicalPosition.BOTTOM_LEFT,
      status: 'info',
      icon: 'copy-outline',
      iconPack: 'eva'
    });
  }

  deleteApplication(application: ApplicationDto) {
    if (application !== null && application !== undefined) {
      this.dialogService.open(ConfirmDialogComponent, {
        autoFocus: false,
        closeOnEsc: true,
        context: {
          title: 'Warning!',
          message: 'Are you sure you want to delete the Application "' + application.name + '" ?',
        },
      }).onClose.subscribe(shouldDelete => {
        if (shouldDelete === true) {
          this.deleteApplicationEmitter.emit(application);
        }
      });
    }
  }

}
