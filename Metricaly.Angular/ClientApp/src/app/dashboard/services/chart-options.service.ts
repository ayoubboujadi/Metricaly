import { Injectable } from '@angular/core';
import { WidgetSettings } from '../models/widget-settings.model';
import { ChartOptions, CommonAxe, ChartYAxe } from 'chart.js';

import { formatDate } from "@angular/common";
import { AxisSettings } from '../models/axis-settings.model';


@Injectable({
  providedIn: 'root'
})
export class ChartOptionsService {

  constructor() { }


  public getDefaultChartOptions(): ChartOptions {
    var chartOptions: ChartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      tooltips: {
        position: "nearest",
        mode: "index",
        intersect: false
      },
      elements: {
        line: {
          borderJoinStyle: 'round',
          borderWidth: 2
        }
      },
      scales: {
        xAxes: [
          {
            offset: false,
            type: "time",
            distribution: "series",
            time: {
              parser: this.dateTickFormatting
            }
          }
        ],
        yAxes: [
          {
            id: 'left',
            type: 'linear',
            position: 'left',
          },
          {
            id: 'right',
            type: 'linear',
            position: 'right',
          }
        ]
      }
    };

    return chartOptions;
  }

  public getChartOptionsFromWidgetSettings(widgetSettings: WidgetSettings, showLeftYAxis: boolean, showRightYAxis: boolean): ChartOptions {

    var lineChartOptions = this.getDefaultChartOptions()

    lineChartOptions.title = {
      display: widgetSettings.displayTitle,
      text: widgetSettings.title
    }
    
    lineChartOptions.scales.xAxes[0].gridLines = {
      display: widgetSettings.xAxisSettings.displayGridLines
    }

    lineChartOptions.scales.xAxes[0].scaleLabel = {
      display: widgetSettings.xAxisSettings.displayLabel,
      labelString: widgetSettings.xAxisSettings.label
    }

    // Set left Y axis settings
    this.setYAxisSettings(widgetSettings.yLeftAxisSettings, lineChartOptions.scales.yAxes.find(a => a.id == 'left'), showLeftYAxis)

    // Set right Y axis settings
    this.setYAxisSettings(widgetSettings.yRightAxisSettings, lineChartOptions.scales.yAxes.find(a => a.id == 'right'), showRightYAxis)

    return lineChartOptions
  }


  private setYAxisSettings(yAxisSettings: AxisSettings, chartYAxe: ChartYAxe, display: boolean) {
    chartYAxe.gridLines = {
      display: yAxisSettings.displayGridLines,
    }

    chartYAxe.scaleLabel = {
      display: yAxisSettings.displayLabel,
      labelString: yAxisSettings.label
    }

    chartYAxe.display = display
  }


  dateTickFormatting(val: any): string {
    var date = new Date(val * 1000);
    return formatDate(date, "medium", "en-US")
  }

}
