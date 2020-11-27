import { Injectable } from '@angular/core';
import { formatDate } from '@angular/common';
import { EChartOption } from 'echarts';
import * as numeral from 'numeral';
import { LineChartWidget, LineChartPlottedMetric } from '@app/web-api-client';

@Injectable({
  providedIn: 'root'
})
export class ChartOptionsService {

  constructor() { }

  public getDefaultChartOptions(): EChartOption {
    const chartOption: EChartOption = {
      title: {
        //text: 'This is my chart'
      },

      textStyle: {
        fontSize: 11
      },

      tooltip: {
        trigger: 'axis',
        axisPointer: {
          type: 'cross',
          label: {
            backgroundColor: '#6a7985'
          }
        }
      },
      dataZoom: [
        {
          type: 'slider'
        },
        {
          type: 'inside'
        }],
      legend: {
        data: []
      },
      xAxis: {
        splitNumber: 15,
        name: 'Time',
        type: 'time',
        splitLine: {
          show: false,
          color: '#f1f4f9'
        },
        axisLabel: {
          fontSize: 11
        }
      },
      yAxis: [
        {
          nameLocation: 'middle',
          type: 'value',
          name: 'Axis Y Left',
          position: 'left',
          id: 'left',
          show: false,
          nameGap: 50,
          splitLine: {
            lineStyle: {
              color: '#f1f4f9'
            },
          },
          axisLabel: {
            fontSize: 11,
            formatter: function (value) {
              return numeral(value).format('0.0a');
            }
          }
        },
        {
          nameLocation: 'middle',
          type: 'value',
          name: 'Axis Y Right',
          position: 'right',
          id: 'right',
          show: false,
          nameGap: 50,
          splitLine: {
            lineStyle: {
              color: '#f1f4f9'
            },
          },
          axisLabel: {
            fontSize: 11,
            formatter: function (value) {
              return numeral(value).format('0.0a');
            }
          },
        }
      ],
    };

    return chartOption;
  }

  dateTickFormatting(val: any): string {
    const date = new Date(val * 1000);
    return formatDate(date, 'medium', 'en-US');
  }

  mapData(metric: any, labels: number[]) {
    const data: any[] = [];
    metric.data.forEach((value, index) => {
      data.push({ name: labels[index] * 1000, value: [labels[index] * 1000, value ?? '-'] });
    });
    return data;
  }

  getUpdateChartOptions(data: LineChartData, lineChartWidget: LineChartWidget) {
    let plottedMetrics: LineChartPlottedMetric[] = [];
    let plottedMetricsData: PlottedMetricData[] = [];
    let labels: number[] = [];
    const widgetSettings = lineChartWidget.widgetSettings;

    if (lineChartWidget !== undefined && lineChartWidget != null && lineChartWidget.plottedMetrics.length !== 0) {
      plottedMetrics = lineChartWidget.plottedMetrics;
    }

    if (data !== undefined && data != null && data.plottedMetricsData.length !== 0) {
      plottedMetricsData = data.plottedMetricsData;
    }

    if (data !== undefined && data != null && data.timestamps.length !== 0) {
      labels = data.timestamps;
    }

    const updateOptions: any = {
      // title: {
      //   text: widgetSettings.title
      // },

      xAxis: {
        name: widgetSettings.xAxisSettings.label,
        type: 'time',
      },

      yAxis: [
        {
          id: 'left',
          show: plottedMetrics.some(metric => metric.yAxis === 'left'),
          name: widgetSettings.yLeftAxisSettings.label,
          //min: 'dataMin'
        },
        {
          id: 'right',
          show: plottedMetrics.some(metric => metric.yAxis === 'right'),
          name: widgetSettings.yRightAxisSettings.label,
        }
      ],

      legend: {
        data: plottedMetrics.map(metric => metric.label),
        show: widgetSettings.displayLegend,
        //align: 'left'
        //orient: 'vertical'
      },

      series: plottedMetricsData.map(metricData => ({
        yAxisIndex: plottedMetrics.find(x => x.guid === metricData.metricGuid).yAxis === 'left' ? 0 : 1,
        type: 'line',
        data: this.mapData(metricData, labels),
        name: plottedMetrics.find(x => x.guid === metricData.metricGuid).label,
        id: metricData.metricGuid,
        //showSymbol: false,
        color: plottedMetrics.find(x => x.guid === metricData.metricGuid).color,
        smooth: widgetSettings.smoothLines,
        markLine: {
          data: [
            { name: 'Max value', yAxis: 10000 }
          ],
        }
      }))
    };


    // Set filled value
    if (widgetSettings.filled) {
      updateOptions.series.forEach(s => s.areaStyle = { opacity: 0.2 });
    } else {
      updateOptions.series.forEach(s => s.areaStyle = null);
    }

    return updateOptions;
  }

  getUpdateChartDataOnly(data: LineChartData) {
    let plottedMetricsData: PlottedMetricData[] = [];
    let labels: number[] = [];

    if (data !== undefined && data != null && data.plottedMetricsData.length !== 0) {
      plottedMetricsData = data.plottedMetricsData;
    }

    if (data !== undefined && data != null && data.timestamps.length !== 0) {
      labels = data.timestamps;
    }

    const updateOptions: any = {
      series: plottedMetricsData.map(metricData => ({
        data: this.mapData(metricData, labels),
        type: 'line',
        id: metricData.metricGuid,
      }))
    };

    return updateOptions;
  }

}

export class LineChartData {
  public plottedMetricsData: PlottedMetricData[] = [];
  public timestamps: number[] = [];
  public samplingTime: number;
}

export class PlottedMetricData {
  public metricGuid: string;
  public data: number[] = [];
}
