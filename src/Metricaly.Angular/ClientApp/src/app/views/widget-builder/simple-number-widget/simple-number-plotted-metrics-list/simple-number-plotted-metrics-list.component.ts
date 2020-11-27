import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { NbDialogService } from '@nebular/theme';
import { SimpleNumberPlottedMetric } from '@app/web-api-client';

@Component({
  selector: 'app-simple-number-plotted-metrics-list',
  templateUrl: './simple-number-plotted-metrics-list.component.html',
  styleUrls: ['./simple-number-plotted-metrics-list.component.css']
})
export class SimpleNumberPlottedMetricsListComponent implements OnInit {

  @Output() plottedMetricsChange: EventEmitter<SimpleNumberPlottedMetric[]> = new EventEmitter;
  @Output() plottedMetricRemovedEvent: EventEmitter<SimpleNumberPlottedMetric> = new EventEmitter;
  @Output() samplingTypeChange: EventEmitter<SimpleNumberPlottedMetric> = new EventEmitter;
  @Input() plottedMetrics: SimpleNumberPlottedMetric[] = [];

  constructor(public dialogService: NbDialogService) { }

  ngOnInit(): void {
  }

  removeMetric(metric: SimpleNumberPlottedMetric) {
    this.plottedMetrics.splice(this.plottedMetrics.indexOf(metric), 1);
    this.plottedMetricRemovedEvent.emit(metric);
  }

  updatePlottedMetrics() {
    this.plottedMetricsChange.emit(this.plottedMetrics);
  }

  handleSamplingTypeChange(metric: SimpleNumberPlottedMetric) {
    this.samplingTypeChange.emit(metric);
  }

}
