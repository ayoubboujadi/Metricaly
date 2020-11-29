import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { NbDialogService } from '@nebular/theme';
import { ColorPickerDialogComponent } from '@app/views/shared/color-picker-dialog/color-picker-dialog.component';
import { LineChartPlottedMetric } from '@app/web-api-client';

@Component({
  selector: 'app-line-chart-plotted-metrics-list',
  templateUrl: './line-chart-plotted-metrics-list.component.html',
  styleUrls: ['./line-chart-plotted-metrics-list.component.css']
})
export class LineChartPlottedMetricsListComponent implements OnInit {

  @Output() plottedMetricsChange: EventEmitter<LineChartPlottedMetric[]> = new EventEmitter;
  @Output() plottedMetricRemovedEvent: EventEmitter<LineChartPlottedMetric> = new EventEmitter;
  @Output() samplingTypeChange: EventEmitter<LineChartPlottedMetric> = new EventEmitter;
  @Input() plottedMetrics: LineChartPlottedMetric[] = [];

  constructor(public dialogService: NbDialogService) { }

  ngOnInit(): void {
  }

  removeMetric(metric: LineChartPlottedMetric) {
    this.plottedMetrics.splice(this.plottedMetrics.indexOf(metric), 1);
    this.plottedMetricRemovedEvent.emit(metric);
  }

  updatePlottedMetrics() {
    this.plottedMetricsChange.emit(this.plottedMetrics);
  }

  handleSamplingTypeChange(metric: LineChartPlottedMetric) {
    this.samplingTypeChange.emit(metric);
  }

  openColorPickerDialog(metric: LineChartPlottedMetric) {
    this.dialogService.open(ColorPickerDialogComponent, { context: { color: metric.color ?? '#ffffff' } })
      .onClose.subscribe(result => {
        if (result) {
          metric.color = result;
          this.updatePlottedMetrics();
        }
      });
  }

  handleStackedChange(metric: LineChartPlottedMetric, newValue: boolean) {
    metric.stacked = newValue;
    this.plottedMetricsChange.emit(this.plottedMetrics);
  }

}
