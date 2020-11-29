import { Component, OnInit, ViewChild, OnDestroy, Input } from '@angular/core';
import { GuidGenerator } from '@app/core/shared/utils/guid-generator';
import { LineChartWidgetComponent } from '@app/views/widgets/line-chart-widget/line-chart-widget.component';
import { DummyDataGeneratorService } from '@app/core/dashboard/services/dummy-data-generator.service';
import { ColorsGeneratorService } from '@app/core/dashboard/services/colors-generator.service';
import { TimePeriod } from '@app/core/shared/models/timeperiod.model';
import { LineChartWidget, LineChartWidgetSettings, LineChartPlottedMetric, MetricDto, MetricClient } from '@app/web-api-client';


@Component({
  selector: 'app-line-chart-builder',
  templateUrl: './line-chart-builder.component.html',
  styleUrls: ['./line-chart-builder.component.css'],
})
export class LineChartBuilderComponent implements OnInit, OnDestroy {
  @ViewChild(LineChartWidgetComponent) lineChartWidgetChild: LineChartWidgetComponent;

  public _lineChartWidget: LineChartWidget;
  @Input() applicationId: string;

  @Input()
  public set lineChartWidget(newValue: LineChartWidget) {
    this._lineChartWidget = newValue;
    this.lineChartWidgetChild?.hardReloadPlottedMetrics();
  }

  public get lineChartWidget(): LineChartWidget {
    return this._lineChartWidget;
  }

  _selectedDaterange: TimePeriod = new TimePeriod(null, null);
  private timer: any;
  loadingMetrics = false;
  applicationMetrics: MetricDto[] = [];

  @Input()
  public set selectedDaterange(newValue: any) {
    this._selectedDaterange.start = newValue.start.unix();
    this._selectedDaterange.end = newValue.end ? newValue.end.unix() : null;
    this.lineChartWidgetChild?.hardReloadPlottedMetrics();
  }

  public get selectedDaterange() {
    return this._selectedDaterange;
  }

  samplingValues: any[] = [
    { label: '1 second', value: 1 },
    { label: '10 seconds', value: 10 },
    { label: '30 seconds', value: 30 },
    { label: '1 min', value: 60 },
    { label: '5 min', value: 300 },
    { label: '15 min', value: 900 },
    { label: '1 hour', value: 3600 },
  ];

  constructor(private dataGenerator: DummyDataGeneratorService, private colorsGenerator: ColorsGeneratorService,
    private metricClient: MetricClient) {
  }


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
      if (this._lineChartWidget?.plottedMetrics && this._lineChartWidget?.plottedMetrics?.length > 0) {
        this.lineChartWidgetChild.loadPlottedMetricsData();
      }
    }, 1000);
  }

  ngOnDestroy() {
    clearInterval(this.timer);
  }

  updateWidgetSettings(newWidgetSettings: LineChartWidgetSettings) {
    this._lineChartWidget.widgetSettings = newWidgetSettings;
    if (this.lineChartWidgetChild) {
      this.lineChartWidgetChild.updateChart();
    }
  }

  metricAddedEvent(metric: MetricDto) {
    const plottedMetric = LineChartPlottedMetric.fromJS(
      {
        metricId: metric.id,
        metricName: metric.name,
        namespace: metric.namespace,
        label: metric.namespace + ' ' + metric.name,
        yAxis: 'left',
        samplingType: 'Average',
        guid: GuidGenerator.newGuid(),
        stacked: false
      });

    plottedMetric.color = this.colorsGenerator.getColor(this._lineChartWidget.plottedMetrics.map(m => m.color));
    //plottedMetric.label = plottedMetric.label + ' ' + plottedMetric.guid;

    this._lineChartWidget.plottedMetrics.push(plottedMetric);
    this.lineChartWidgetChild?.hardReloadPlottedMetrics();

    // var startTime = (Date.now() / 1000) - (3600 * 0.5)
    // var newData = this.dataGenerator.getData(startTime, this.lineChartWidget.plottedMetrics.map(x => x.guid))
    //this.lineChartData.plottedMetricsData = newData.metricsTimeSeriesData.map(x => {
    //  let data = new PlottedMetricData
    //  data.metricGuid = x.metricGuid
    //  data.data = x.data
    //  return data
    //})
    //this.lineChartData.timestamps = newData.timestamps

    //if (this.lineChartWidgetChild)
    //  this.lineChartWidgetChild.updateChart();
  }

  plottedMetricsChanged(plottedMetrics: LineChartPlottedMetric[]) {
    console.log('plottedMetricsChanged: ' + plottedMetrics.length);
    // Update the chart with the new plottedMetrics
    this._lineChartWidget.plottedMetrics = plottedMetrics;
    if (this.lineChartWidgetChild) {
      this.lineChartWidgetChild.updateChart();
    }
  }

  plottedMetricRemoved(removedMetric: LineChartPlottedMetric) {
    // When a metric is removed from the graph, the graph should be refreshed first
    this.lineChartWidgetChild.hardReloadPlottedMetrics();
    this.lineChartWidgetChild.refreshChart();
  }

  handleSamplingTimeChanged(newValue: number) {
    // Update the chart when the sampling time value changes
    this.lineChartWidgetChild.hardReloadPlottedMetrics();
  }

  handleSamplingTypeChange(metric: LineChartPlottedMetric) {
    this.lineChartWidgetChild.hardReloadPlottedMetrics();
  }

  hasPlottedMetrics() {
    return this._lineChartWidget && this._lineChartWidget.plottedMetrics && this._lineChartWidget.plottedMetrics.length > 0;
  }
}
