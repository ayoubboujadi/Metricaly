
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// Components
import { CoreModule } from '@app/core/core.module';
import { WidgetsManageComponent } from './widgets-manage/widgets-manage.component';
import { WidgetCreateComponent } from './widget-create/widget-create.component';

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
  declarations: [WidgetCreateComponent, WidgetsManageComponent],
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

    CoreModule,

    RouterModule.forChild([
      {
        path: '',
        redirectTo: 'create'
      },
      {
        path: 'create',
        component: WidgetCreateComponent,
        data: { breadcrumb: 'Create Widget' }
      },
      {
        path: 'manage',
        component: WidgetsManageComponent,
        data: { breadcrumb: 'Manage Widgets' }
      },
    ])
  ]
})
export class WidgetPageModule { }
