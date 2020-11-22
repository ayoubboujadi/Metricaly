import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Application } from '../../../views/pages/application/application.component';
import { MetricsDataRequest, Metric, MetricTimeSeriesResponse } from '../../../core/dashboard/services/line-series-data.service';
import { PlottedMetricData } from '../../../core/dashboard/services/chart-options.service';

@Injectable({
  providedIn: 'root'
})
export class LineChartWidgetService {

  constructor(private http: HttpClient) { }

  parseRequest(): MetricsDataRequest {
    let data = new MetricsDataRequest

    data.applicationId = this.lineChartWidget.applicationId
    data.startTimestamp = this.timePeriod.start
    data.endTimestamp = this.timePeriod.end

    data.samplingTime = this.lineChartWidget.samplingTime

    this.lineChartWidget.plottedMetrics?.forEach(x => {
      var metric = new Metric
      metric.Guid = x.guid
      metric.metricName = x.metricName
      metric.namespace = x.namespace
      metric.samplingType = +x.samplingType
      data.metrics.push(metric)
    })

    return data;
  }

  loadPlottedMetricsData() {
    if (this.lineChartWidget.plottedMetrics && this.lineChartWidget.plottedMetrics.length != 0) {

      let data = this.parseRequest()

      this.dataService.getMetricsData(data).subscribe((data: MetricTimeSeriesResponse) => {
        this.data.plottedMetricsData = data.values.map(x => {
          let data = new PlottedMetricData
          data.metricGuid = x.guid
          data.data = x.values
          return data
        })
        this.data.timestamps = data.timestamps
        this.updateDataOnly()
      })
    }
  }

  hardReloadPlottedMetrics() {
    this.loading = true;
    let data = this.parseRequest()

    if (data && data.metrics && data.metrics.length > 0) {
      this.dataService.getMetricsData(data).subscribe((data: MetricTimeSeriesResponse) => {
        this.data.plottedMetricsData = data.values.map(x => {
          let data = new PlottedMetricData
          data.metricGuid = x.guid
          data.data = x.values
          return data
        })
        this.data.timestamps = data.timestamps
        this.updateChart()
        this.loading = false;
      })
    }
  }

}
