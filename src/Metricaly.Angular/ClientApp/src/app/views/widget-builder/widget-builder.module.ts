import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// Other libraries
import { ColorSketchModule } from 'ngx-color/sketch';
import { NgxEchartsModule } from 'ngx-echarts';

// Internal
import { WidgetBuilderComponent } from './widget-builder.component';
import { LineChartSettingsFormComponent } from './line-chart-widget/line-chart-settings-form/line-chart-settings-form.component';
import { LineChartBuilderComponent } from './line-chart-widget/line-chart-builder/line-chart-builder.component';
import { WidgetsModule } from '../widgets/widgets.module';
import { SharedModule } from '../shared/shared.module';

// Nebular
import { NbThemeModule, NbCardModule, NbTabsetModule, NbListModule, NbButtonModule, NbIconModule, NbDialogModule, NbInputModule, NbToggleModule, NbMenuModule, NbSelectModule, NbFormFieldModule, NbCalendarKitModule, NbSpinnerModule, NbTooltipModule } from '@nebular/theme';
import { MetricsListComponent } from './metrics-list/metrics-list.component';
import { LineChartPlottedMetricsListComponent } from './line-chart-widget/line-chart-plotted-metrics-list/line-chart-plotted-metrics-list.component';


@NgModule({
  declarations: [WidgetBuilderComponent, LineChartBuilderComponent, LineChartSettingsFormComponent, MetricsListComponent, LineChartPlottedMetricsListComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,

    ColorSketchModule,
    NgxEchartsModule.forChild(),

    WidgetsModule,
    SharedModule,

    NbThemeModule,
    NbCardModule,
    NbTabsetModule,
    NbListModule,
    NbButtonModule,
    NbIconModule,
    NbInputModule,
    NbToggleModule,
    NbDialogModule.forChild(),
    NbMenuModule,
    NbSelectModule,
    NbFormFieldModule,
    NbSpinnerModule,
    NbSelectModule,
    NbTooltipModule,

    RouterModule.forChild([
      {
        path: 'line-chart/:widgetId',
        component: WidgetBuilderComponent,
        data: {breadcrumb: 'Widget Builder'}
      },
    ])
  ]
})
export class WidgetBuilderModule { }
