import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { MetricData } from '../../../core/shared/models/metric-data.model';

@Component({
  selector: 'app-metrics-list',
  templateUrl: './metrics-list.component.html',
  styleUrls: ['./metrics-list.component.css']
})
export class MetricsListComponent implements OnInit {

  @Output() metricAddedEvent: EventEmitter<MetricData> = new EventEmitter

  @Input()
  public metrics: MetricData[] = [
    //{ id: "1", name: "Metric 1", namespace: "Namespace 1" },
    //{ id: "2", name: "Metric 2", namespace: "Namespace 1" },
    //{ id: "3", name: "Metric 3", namespace: "Namespace 1" },
    //{ id: "4", name: "Metric 4", namespace: "Namespace 1" },
  ]

  constructor() { }

  ngOnInit(): void {
  }

  addMetricToPlot(metricData: MetricData) {
    //metricData.isPlotted = true;
    this.metricAddedEvent.emit(metricData);
  }

}
