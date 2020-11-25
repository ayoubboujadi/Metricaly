import { Component, OnInit, Input } from '@angular/core';
import { EChartOption } from 'echarts';
import { ChartOptionsService, LineChartData, PlottedMetricData } from '@app/core/dashboard/services/chart-options.service';
import { LineSeriesData } from '@app/core/dashboard/services/line-series-data.service';
import { TimePeriod } from '@app/core/shared/models/timeperiod.model';
import { LineChartWidget, MetricClient, GetMetricTimeSeriesQuery, MetricNamespaceDTO } from '@app/web-api-client';

@Component({
  selector: 'app-line-chart-widget',
  templateUrl: './line-chart-widget.component.html',
  styleUrls: ['./line-chart-widget.component.css']
})
export class LineChartWidgetComponent implements OnInit {

  _lineChartWidget: LineChartWidget;
  @Input() set lineChartWidget(value: LineChartWidget) {
    this._lineChartWidget = value;
    this.hardReloadPlottedMetrics();
  }


  _timePeriod: TimePeriod;
  @Input() set timePeriod(value: TimePeriod) {
    this._timePeriod = value;
    this.hardReloadPlottedMetrics();
  }

  @Input() loading = false;
  @Input() applicationId: string;
  data: LineChartData = new LineChartData;
  chartOptions: EChartOption;
  updateOptions: any;
  echartsInstance: any;

  constructor(private chartOptionsService: ChartOptionsService, private metricClient: MetricClient) {
    this.chartOptions = this.chartOptionsService.getDefaultChartOptions();
  }

  ngOnInit(): void {
  }

  onChartInit(ec) {
    this.echartsInstance = ec;
  }

  updateChart() {
    // Update the chart
    this.updateOptions = this.chartOptionsService.getUpdateChartOptions(this.data, this._lineChartWidget);
  }

  updateDataOnly() {
    // Update the chart
    this.updateOptions = this.chartOptionsService.getUpdateChartDataOnly(this.data);
  }

  refreshChart() {
    if (this.echartsInstance) {
      // Temp fix for bug when you remove a data series but it still displays
      // on the graph, see: https://github.com/apache/incubator-echarts/issues/6202
      this.echartsInstance.clear();
      this.echartsInstance.setOption(this.chartOptions);
    }
  }


  parseRequest(): GetMetricTimeSeriesQuery {
    return GetMetricTimeSeriesQuery.fromJS({
      startTimestamp: this._timePeriod.start,
      endTimestamp: this._timePeriod.end,
      samplingTime: this._lineChartWidget.samplingTime,
      applicationId: this.applicationId,
      metrics: this._lineChartWidget.plottedMetrics?.map(x => {
        return MetricNamespaceDTO.fromJS({
          guid: x.guid,
          metricName: x.metricName,
          namespace: x.namespace,
          samplingType: x.samplingType
        });
      })
    });
  }

  loadPlottedMetricsData() {
    if (this._lineChartWidget?.plottedMetrics && this._lineChartWidget.plottedMetrics?.length !== 0 && this._timePeriod) {
      this.metricClient.getMetricValues(this.parseRequest())
        .subscribe(result => {
          this.data.plottedMetricsData = result.values.map(x => {
            const data = new PlottedMetricData;
            data.metricGuid = x.guid;
            data.data = x.values;
            return data;
          });
          this.data.timestamps = result.timestamps;
          this.updateDataOnly();
        });
    }
  }

  hardReloadPlottedMetrics() {
    if (this._lineChartWidget?.plottedMetrics && this._lineChartWidget?.plottedMetrics.length !== 0 && this._timePeriod) {
      this.loading = true;
      this.metricClient.getMetricValues(this.parseRequest())
        .subscribe(result => {

          this.data.plottedMetricsData = result.values.map(x => {
            const data = new PlottedMetricData;
            data.metricGuid = x.guid;
            data.data = x.values;
            return data;
          });
          this.data.timestamps = result.timestamps;
          this.updateChart();
          this.loading = false;
        });
    }
  }
}


