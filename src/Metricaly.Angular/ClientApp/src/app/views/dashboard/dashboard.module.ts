import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// My components
import { DashboardComponent } from './dashboard.component';
import { RouterModule } from '@angular/router';
import { GridItemComponent } from './grid-item/grid-item.component';
import { GridComponent } from './grid/grid.component';

// Others
import { ColorSketchModule } from 'ngx-color/sketch';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// Nebular
import {
  NbThemeModule,
  NbCardModule,
  NbTabsetModule,
  NbListModule,
  NbButtonModule,
  NbIconModule,
  NbDialogModule,
  NbInputModule,
  NbToggleModule,
  NbMenuModule,
  NbSpinnerModule,
  NbBadgeModule,
  NbToastrModule,
  NbSelectModule,
  NbTooltipModule,
  NbContextMenuModule
} from '@nebular/theme';

import { NgxEchartsModule } from 'ngx-echarts';
import { WidgetsModule } from '../widgets/widgets.module';
import { DashboardItemComponent } from './dashboard-item/dashboard-item.component';
import { SharedModule } from '../shared/shared.module';
import { WidgetsOverviewComponent } from './widgets-overview/widgets-overview.component';

@NgModule({
  declarations: [DashboardComponent, GridItemComponent, GridComponent, DashboardItemComponent, WidgetsOverviewComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,

    NbThemeModule,
    NbCardModule,
    NbTabsetModule,
    NbListModule,
    NbButtonModule,
    NbIconModule,
    NbInputModule,
    NbToggleModule,
    NbMenuModule,
    NbSpinnerModule,
    NbBadgeModule,
    NbToastrModule,
    NbDialogModule.forChild(),
    NbSelectModule,
    NbTooltipModule,
    NbContextMenuModule,

    SharedModule,
    WidgetsModule,
    ColorSketchModule,
    NgxEchartsModule.forChild(),

    RouterModule.forChild([
      {
        path: 'view/:dashboardId',
        component: DashboardComponent,
        data: { breadcrumb: '', hideBreadcrumb: true }
      },
    ])
  ]
})
export class DashboardModule { }
