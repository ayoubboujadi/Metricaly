import { Component, OnInit, ViewChild, OnDestroy, Input } from '@angular/core';
import { SimpleNumberWidgetComponent } from '@app/views/widgets/simple-number-widget/simple-number-widget.component';
import { TimePeriod } from '@app/core/shared/models/timeperiod.model';
import { SimpleNumberWidget, MetricDto, MetricClient, SimpleNumberPlottedMetric } from '@app/web-api-client';
import { GuidGenerator } from '@app/core/shared/utils/guid-generator';
import { ColorsGeneratorService } from '@app/core/dashboard/services/colors-generator.service';

@Component({
  selector: 'app-simple-number-builder',
  templateUrl: './simple-number-builder.component.html',
  styleUrls: ['./simple-number-builder.component.css']
})
export class SimpleNumberBuilderComponent implements OnInit, OnDestroy {

  @ViewChild(SimpleNumberWidgetComponent) widgetChildElement: SimpleNumberWidgetComponent;

  @Input() applicationId: string;

  public _widgetData: SimpleNumberWidget;

  @Input()
  public set widgetData(newValue: SimpleNumberWidget) {
    this._widgetData = newValue;
    this.widgetChildElement?.hardReloadPlottedMetrics();
  }

  public get widgetData(): SimpleNumberWidget {
    return this._widgetData;
  }

  _selectedDaterange: TimePeriod = new TimePeriod(null, null);
  @Input()
  public set selectedDaterange(newValue: any) {
    this._selectedDaterange.start = newValue.start.unix();
    this._selectedDaterange.end = newValue.end ? newValue.end.unix() : null;
    this.widgetChildElement?.hardReloadPlottedMetrics();
  }

  public get selectedDaterange() {
    return this._selectedDaterange;
  }

  private timer: any;
  loadingMetrics = false;
  applicationMetrics: MetricDto[] = [];

  constructor(private metricClient: MetricClient, private colorsGenerator: ColorsGeneratorService) { }

  ngOnInit(): void {
    this.loadingMetrics = true;
    // Load the widget's application's metrics
    this.metricClient.listMetrics(this.applicationId)
      .subscribe((result) => {
        this.applicationMetrics = result;
        this.loadingMetrics = false;
      });

    // Mock dynamic data:
    this.timer = setInterval(() => {
      if (this._widgetData?.plottedMetrics && this._widgetData?.plottedMetrics?.length > 0) {
        this.widgetChildElement.loadPlottedMetricsData();
      }
    }, 2000);
  }

  ngOnDestroy() {
    clearInterval(this.timer);
  }

  metricAddedEvent(metric: MetricDto) {
    const plottedMetric = SimpleNumberPlottedMetric.fromJS(
      {
        metricId: metric.id,
        guid: GuidGenerator.newGuid(),
        label: metric.namespace + ' ' + metric.name,
        unit: 'unit',
        color: this.colorsGenerator.getColor(this.widgetData.plottedMetrics.map(m => m.color)),
        metricName: metric.name,
        namespace: metric.namespace,
        samplingType: 'Average',
      });

    this._widgetData.plottedMetrics.push(plottedMetric);
    this.widgetChildElement?.hardReloadPlottedMetrics();
  }

  plottedMetricsChanged(plottedMetrics: SimpleNumberPlottedMetric[]) {
    this.widgetData.plottedMetrics = plottedMetrics;
    this.widgetChildElement.hardReloadPlottedMetrics();
  }

  plottedMetricRemoved(removedMetric: SimpleNumberPlottedMetric) {
    this.widgetChildElement.hardReloadPlottedMetrics();
  }

  handleSamplingTypeChange(metric: SimpleNumberPlottedMetric) {
    this.widgetChildElement.hardReloadPlottedMetrics();
  }

  hasPlottedMetrics() {
    return this._widgetData && this._widgetData.plottedMetrics && this._widgetData.plottedMetrics.length > 0;
  }

}
