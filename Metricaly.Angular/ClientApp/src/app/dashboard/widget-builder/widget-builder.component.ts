import { Component, OnInit } from '@angular/core';
import { multi } from '../data';

import { ChartDataSets, ChartOptions } from 'chart.js';
import { Color, Label } from 'ng2-charts';
import { WidgetSettings } from '../models/widget-settings.model';
import { ChartOptionsService } from '../services/chart-options.service';
import { ColorsGeneratorService } from '../services/colors-generator.service';
//import { MatDialog } from '@angular/material/dialog';
import { ColorPickerDialogComponent } from '../color-picker-dialog/color-picker-dialog.component';
import { NbDialogService } from '@nebular/theme';


@Component({
  selector: 'app-widget-builder',
  templateUrl: './widget-builder.component.html',
  styleUrls: ['./widget-builder.component.css']
})
export class WidgetBuilderComponent implements OnInit {


  //public lineChartData: ChartDataSets[] = [
  //{ data: multi[0].series.map(x => x.value), label: 'Series A', yAxisID: 'A' },
  //{ data: multi[0].series.map(x => Math.floor(Math.random() * Math.floor(5000))), label: 'Random Series', yAxisID: 'B' }
  //];

  public plottedMetrics: Metric[] = []
  public allMetrics: MetricData[] = [
    { id: "1", name: "Metric 1", namespace: "Namespace 1", isPlotted: false },
    { id: "2", name: "Metric 2", namespace: "Namespace 1", isPlotted: false },
    { id: "3", name: "Metric 3", namespace: "Namespace 1", isPlotted: false },
    { id: "4", name: "Metric 4", namespace: "Namespace 1", isPlotted: false },
  ]

  public lineChartData: ChartDataSets[] = []


  public lineChartLabels: any = multi[0].series.map(x => x.name);

  public lineChartOptions: ChartOptions
  public lineChartColors: Color[] = [
    //{
    //  borderColor: 'rgba(255,0,0,0.7)',
    //  backgroundColor: 'rgba(0,0,0,0)',
    //  pointBackgroundColor: 'rgba(255,0,0,0.7)'
    //  //backgroundColor: 'rgba(255,0,0,0.3)',
    //},
    //{
    //  borderColor: 'rgba(174,218,247,0.7)',
    //  backgroundColor: 'rgba(0,0,0,0)',
    //  pointBackgroundColor: 'rgba(174,218,247,0.7)'
    //  //backgroundColor: 'rgba(255,0,0,0.3)',
    //},
  ];
  public lineChartLegend = true;
  public lineChartType = 'line';
  public lineChartPlugins = [];

  public widgetSettings: WidgetSettings

  // 1. Load all the metrics from the DB
  // 2. Show all namespaces and their metrics
  // 3. When user clicks add button on a metric, it gets added to another list to be plotted
  // 4. User can change settings of this metric (label, yaxis ...)


  constructor(private chartOptionsService: ChartOptionsService, private colorsGenerator: ColorsGeneratorService
    , public dialogService: NbDialogService
  ) {
    this.widgetSettings = new WidgetSettings
    this.widgetSettings.title = "This is my widget"

    this.lineChartOptions = this.chartOptionsService.getDefaultChartOptions()
  }

  ngOnInit(): void {

  }

  addMetric() {
    console.log("clicked")
    console.log(JSON.stringify(this.widgetSettings))
    //this.lineChartData.push({ data: multi[0].series.map(x => x.value), label: 'Series A', yAxisID: 'A' })

    //var metric = new Metric
    //metric.color = 'rgba(255,0,0,0.7)';
    //metric.label = "Metric 1"
    //metric.data = multi[0].series.map(x => Math.floor(Math.random() * Math.floor(5000)))
    //metric.yAxis = 'left'

    //this.plottedMetrics.push(metric)
    //this.lineChartData = this.plottedMetrics.map(metric => ({ data: metric.data, label: metric.label, yAxisID: metric.yAxis }))
  }


  addMetricToPlot(metricData: MetricData) {
    console.log("addMetricToPlot")
    if (this.plottedMetrics.some(m => m.id == metricData.id))
      return;

    var metric = new Metric(metricData)

    var max = Math.floor(Math.random() * Math.floor(10000))
    metric.data = multi[0].series.map(x => Math.floor(Math.random() * Math.floor(max)))
    metric.color = this.colorsGenerator.getColor(this.plottedMetrics.map(m => m.color))

    this.plottedMetrics.push(metric)
    metricData.isPlotted = true;

    this.updateLineChartData()
  }

  removeMetricFromPlot(metric: Metric) {

    var metricData = this.allMetrics.find(m => m.id == metric.id)
    metricData.isPlotted = false

    this.plottedMetrics.splice(this.plottedMetrics.indexOf(metric), 1)
    this.updateLineChartData()
  }

  updateLineChartData() {
    console.log(" updateLineChartData()")
    this.lineChartData = this.plottedMetrics.map(metric => ({
      data: metric.data,
      label: metric.label,
      yAxisID: metric.yAxis,
      lineTension: 0,
      pointBorderWidth: 0,
      fill: false,
      backgroundColor: metric.color,
      borderColor: metric.color,
      //pointBackgroundColor: "",
      //pointBorderColor: "",
      pointRadius: 2,
      pointHitRadius: 2
    }))
    this.updateChartOptions()
  }

  updateWidgetSettings(newWidgetSettings: WidgetSettings) {
    console.log(" updateWidgetSettings()")
    this.widgetSettings = newWidgetSettings;
    this.updateChartOptions()
  }

  updateChartOptions() {
    let displayYLeftAxis = this.plottedMetrics.some(metric => metric.yAxis == "left")
    let displayYRightAxis = this.plottedMetrics.some(metric => metric.yAxis == "right")
    this.lineChartOptions = this.chartOptionsService.getChartOptionsFromWidgetSettings(this.widgetSettings, displayYLeftAxis, displayYRightAxis)
  }

  openColorDialog(metric: Metric) {
    this.dialogService.open(ColorPickerDialogComponent, { context: { color: metric.color }})
      .onClose.subscribe(result => {
        if (result) {
          metric.color = result
          this.updateLineChartData()
        }
      });

    //const dialogRef = this.dialog.open(ColorPickerDialogComponent, {
    //  width: '250px',
    //  data: metric.color
    //});

    //dialogRef.afterClosed().subscribe(result => {
    //  if (result != null && result != undefined) {
    //    metric.color = result;
    //    this.updateLineChartData()
    //  }
    //});
  }
}


export class Metric {

  constructor(metricData: MetricData) {
    this.id = metricData.id
    this.label = metricData.namespace + " " + metricData.name
    this.yAxis = "left"
  }

  public id: string
  public label: string
  public data: number[]
  public color: string
  public yAxis: string // left or right

}

export class MetricData {
  public id: string
  public name: string
  public namespace: string
  public isPlotted: boolean = false;
}



