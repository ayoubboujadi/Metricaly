import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LineChartWidgetComponent } from './line-chart-widget/line-chart-widget.component';
import { NgxEchartsModule } from 'ngx-echarts';



@NgModule({
  declarations: [LineChartWidgetComponent],
  imports: [
    CommonModule,
    NgxEchartsModule.forChild(),
  ],
  exports: [
    LineChartWidgetComponent
  ]
})
export class WidgetsModule { }
