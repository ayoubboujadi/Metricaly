import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from './../../shared/shared.module';

// Components
import { CoreModule } from '@app/core/core.module';
import { DashboardCreateComponent } from './dashboard-create/dashboard-create.component';
import { DashboardsManageComponent } from './dashboards-manage/dashboards-manage.component';

// Nebular
import { NbEvaIconsModule } from '@nebular/eva-icons';
import {
  NbCardModule,
  NbSelectModule,
  NbInputModule,
  NbButtonModule,
  NbSpinnerModule,
  NbIconModule,
  NbTooltipModule
} from '@nebular/theme';


@NgModule({
  declarations: [DashboardCreateComponent, DashboardsManageComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,

    NbCardModule,
    NbSelectModule,
    NbInputModule,
    NbButtonModule,
    NbSpinnerModule,
    NbIconModule,
    NbEvaIconsModule,
    NbTooltipModule,

    SharedModule,
    CoreModule,

    RouterModule.forChild([
      {
        path: '',
        redirectTo: 'create'
      },
      {
        path: 'create',
        component: DashboardCreateComponent,
        data: { breadcrumb: 'Create Dashboard' }
      },
      {
        path: 'manage',
        component: DashboardsManageComponent,
        data: { breadcrumb: 'Manage Dashboards' }
      }
    ])
  ]
})
export class DashboardPageModule { }
