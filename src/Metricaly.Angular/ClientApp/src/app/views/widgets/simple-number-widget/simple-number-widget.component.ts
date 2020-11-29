import { Component, OnInit, Input } from '@angular/core';
import { TimePeriod } from '@app/core/shared/models/timeperiod.model';
import { WidgetComponent } from '../widget-component.interface';
import * as numeral from 'numeral';

import {
  GetMetricsAggregatedValueQuery,
  MetricClient,
  SimpleNumberPlottedMetric,
  SimpleNumberWidget,
  AggregateMetricRequestDto
} from '@app/web-api-client';

@Component({
  selector: 'app-simple-number-widget',
  templateUrl: './simple-number-widget.component.html',
  styleUrls: ['./simple-number-widget.component.css'],
  providers: [ {provide: WidgetComponent, useExisting: SimpleNumberWidgetComponent }]
})
export class SimpleNumberWidgetComponent implements WidgetComponent, OnInit {

  @Input() applicationId: string;

  _widgetData: SimpleNumberWidget;
  @Input() set widgetData(value: SimpleNumberWidget) {
    this._widgetData = value;
    this.updateData();
    this.hardReloadPlottedMetrics();
  }

  _timePeriod: TimePeriod;
  @Input() set timePeriod(value: TimePeriod) {
    this._timePeriod = value;
    this.hardReloadPlottedMetrics();
  }

  @Input() loading = false;

  data: SimpleNumberWidgetData[] = [];

  constructor(private metricClient: MetricClient) { }

  ngOnInit(): void {
    /*this.data = [
      {
        label: 'Metric 1',
        unit: 'Mb',
        color: '#0095ff',
        samplingType: 'Max',
        value: 52000
      },
      {
        label: 'Metric 2',
        unit: 'Gb',
        color: '#ff00b1',
        samplingType: 'Max',
        value: '52.65M'
      },
      {
        label: 'Total bandwidth',
        unit: 'Gb',
        color: '#33af00',
        samplingType: 'Max',
        value: '2,123 000'
      },
      {
        label: 'Metric 1',
        unit: 'Mb',
        color: '#0095ff',
        samplingType: 'Max',
        value: '52.3k'
      },
      {
        label: 'Metric 2',
        unit: 'Gb',
        color: '#ff00b1',
        samplingType: 'Max',
        value: '52.65M'
      },
      {
        label: 'Total bandwidth',
        unit: 'Gb',
        color: '#33af00',
        samplingType: 'Max',
        value: '2,123 000'
      }
    ];*/
  }

  updateData() {
    const existingGuids = this.data.map(x => x.metric.guid);

    this._widgetData?.plottedMetrics.forEach(plottedMetric => {
      const dataPoint = this.data.find(y => y.metric.guid === plottedMetric.guid);
      if (dataPoint === null || dataPoint === undefined) {
        // Add new plotted metric
        this.data.push({ value: '-', metric: plottedMetric });
      } else {
        // Update existing plotted metric
        dataPoint.metric = plottedMetric;
      }
    });

    if (existingGuids.length > 0) {
      existingGuids.forEach(existingId => {
        const existingMetric = this._widgetData?.plottedMetrics.find(x => x.guid === existingId);
        if (existingMetric === null || existingMetric === undefined) {
          this.data.splice(this.data.findIndex(x => x.metric.guid === existingId), 1);
        }
      });
    }

  }

  parseRequest(): GetMetricsAggregatedValueQuery {
    return GetMetricsAggregatedValueQuery.fromJS({
      startTimestamp: this._timePeriod.start,
      endTimestamp: this._timePeriod.end,
      applicationId: this.applicationId,
      metrics: this._widgetData.plottedMetrics?.map(x => {
        return AggregateMetricRequestDto.fromJS({
          guid: x.guid,
          metricName: x.metricName,
          namespace: x.namespace,
          samplingType: x.samplingType
        });
      })
    });
  }

  loadPlottedMetricsData() {
    if (this._widgetData?.plottedMetrics && this._widgetData.plottedMetrics?.length !== 0 && this._timePeriod) {
      this.metricClient.getAggregatedValue(this.parseRequest())
        .subscribe(result => {
          result.forEach(metricAggregatedValue => {
            const dataPoint = this.data.find(y => y.metric.guid === metricAggregatedValue.guid);
            if (dataPoint !== null || dataPoint !== undefined) {
              dataPoint.value = metricAggregatedValue.value !== null ? numeral(metricAggregatedValue.value).format('0,0.00') : '-';
            }
            this.loading = false;
          });
        });
    }
  }

  hardReloadPlottedMetrics() {
    this.loading = true;
    this.updateData();
    this.loadPlottedMetricsData();
  }

}

export class SimpleNumberWidgetData {
  value: number | string | null;
  metric: SimpleNumberPlottedMetric;
}
