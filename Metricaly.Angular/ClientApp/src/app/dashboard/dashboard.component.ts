import { Component, OnInit, ViewChild } from '@angular/core';
import { GridComponent, Item } from './grid/grid.component';
import { multi } from './data';
import { formatDate } from "@angular/common";

import { ChartDataSets, ChartOptions } from 'chart.js';
import { Color, Label } from 'ng2-charts';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  @ViewChild(GridComponent, { static: true }) stackgrid: GridComponent;

  public items: Item[] = []
  public id = 2;


  public lineChartData: ChartDataSets[] = [
    { data: multi[0].series.map(x => x.value), label: 'Series A', yAxisID: 'A' },
    { data: multi[0].series.map(x => Math.floor(Math.random() * Math.floor(5000))), label: 'Random Series', yAxisID: 'B' }
  ];
  public lineChartLabels: any = multi[0].series.map(x => x.name);

  public lineChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      xAxes: [
        {
          type: "time",
          distribution: "series",
          time: {
            parser: this.dateTickFormatting
          }
        }
      ],
      yAxes: [
        {
          id: 'A',
          type: 'linear',
          position: 'left'
        },
        {
          id: 'B',
          type: 'linear',
          position: 'right'
        }
      ]
    }
  };
  public lineChartColors: Color[] = [
    {
      borderColor: 'rgba(255,0,0,0.7)',
      backgroundColor: 'rgba(0,0,0,0)',
      pointBackgroundColor: 'rgba(255,0,0,0.7)'
      //backgroundColor: 'rgba(255,0,0,0.3)',
    },
    {
      borderColor: 'rgba(174,218,247,0.7)',
      backgroundColor: 'rgba(0,0,0,0)',
      pointBackgroundColor: 'rgba(174,218,247,0.7)'
      //backgroundColor: 'rgba(255,0,0,0.3)',
    },
  ];
  public lineChartLegend = true;
  public lineChartType = 'line';
  public lineChartPlugins = [];




  constructor() {
    var names = multi[0].series.map(x => x.name);
    var values = multi[0].series.map(x => x.value);
    console.log("names " + names)
  }

  ngOnInit() {
    var item1 = new WidgetItem(0, 0, 1, 1, "1")
    var item2 = new WidgetItem(2, 2, 2, 2, "2")

    this.items.push(item1, item2)
  }

  dateTickFormatting(val: any): string {
    var date = new Date(val * 1000);
    console.log("dateTickFormatting:" + date)
    return formatDate(date, "medium", "en-US")
  }


  addWidget() {
    this.id++;
    console.log("addWidget clicked: " + this.id)
    var item1 = new WidgetItem(0, 0, 1, 1, this.id + "")
  }

  showWidgetJson() {
    //var gridItems = this.stackgrid.GridItems;
    //var items = gridItems.map(x => new WidgetItem(x))

    console.log('show widget json: ' + JSON.stringify(this.items))
  }
}

export class Grid {
  public items: WidgetItem[]
}

export class WidgetItem implements Item {
  constructor(x: number, y: number, width: number, height: number, id: string) {
    this.x = x;
    this.y = y;
    this.height = height;
    this.width = width;
    this.id = id;

    this.value = "Item " + id
  }

  autoPosition: boolean;
  locked: boolean;
  maxHeight: number;
  maxWidth: number;
  minHeight: number;
  minWidth: number;
  noMove: boolean;
  noResize: boolean;
  el: any;

  public x: number;
  public y: number;
  public width: number;
  public height: number;
  public value: string;
  public id: string;
}
