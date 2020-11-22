import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// Components
import { CoreModule } from '@app/core/core.module';
import { ApplicationComponent } from './application.component';
import { ApplicationCreateComponent } from './application-create/application-create.component';
import { ApplicationManageComponent } from './application-manage/application-manage.component';

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
  declarations: [ApplicationComponent, ApplicationCreateComponent, ApplicationManageComponent],
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
        component: ApplicationComponent,
        data: { breadcrumb: 'Applications' }
      }
    ])
  ]
})
export class ApplicationModule { }
