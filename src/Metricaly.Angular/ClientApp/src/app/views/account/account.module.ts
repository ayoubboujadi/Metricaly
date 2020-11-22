import { NbThemeModule, NbAlertModule, NbButtonModule, NbLayoutModule, NbCardModule, NbInputModule } from '@nebular/theme';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';

import { SharedModule } from './../shared/shared.module';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { AccountComponent } from './account.component';
import { AccountRoutingModule } from './account.routing';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [RegisterComponent, LoginComponent, AccountComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,

    NbThemeModule,
    NbButtonModule,
    NbAlertModule,
    NbLayoutModule,
    NbCardModule,
    NbInputModule,

    SharedModule,
    AccountRoutingModule,
  ]
})
export class AccountModule {
}
