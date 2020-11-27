import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LineChartWidgetComponent } from './line-chart-widget/line-chart-widget.component';
import { NgxEchartsModule } from 'ngx-echarts';
import { SimpleNumberWidgetComponent } from './simple-number-widget/simple-number-widget.component';
import { NbCardModule, NbSpinnerModule } from '@nebular/theme';

@NgModule({
  declarations: [LineChartWidgetComponent, SimpleNumberWidgetComponent],
  imports: [
    CommonModule,
    NgxEchartsModule.forChild(),

    NbCardModule,
    NbSpinnerModule
  ],
  exports: [
    LineChartWidgetComponent,
    SimpleNumberWidgetComponent
  ]
})
export class WidgetsModule { }
