import { ApplicationDto } from '@app/web-api-client';
import { Component, OnInit, Input } from '@angular/core';
import { NbToastrService, NbGlobalPhysicalPosition } from '@nebular/theme';

@Component({
  selector: 'app-application-manage',
  templateUrl: './application-manage.component.html',
  styleUrls: ['./application-manage.component.css']
})
export class ApplicationManageComponent implements OnInit {

  @Input()
  applications: ApplicationDto[];

  constructor(private toastrService: NbToastrService) { }

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


  deleteApplication(application) {

  }

}
