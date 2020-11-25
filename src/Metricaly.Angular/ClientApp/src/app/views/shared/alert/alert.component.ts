import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

import { AlertService } from '@app/core/shared/services/alert.service';

@Component({ selector: 'alert', templateUrl: 'alert.component.html' })
export class AlertComponent implements OnInit, OnDestroy {
  private subscription: Subscription;
  message: any;

  constructor(private alertService: AlertService) {
  }

  ngOnInit() {
    this.subscription = this.alertService.getAlert()
    .subscribe(message => {
        this.message = message;
      });
  }

  handleAlertClose() {
    this.alertService.clear();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

}
