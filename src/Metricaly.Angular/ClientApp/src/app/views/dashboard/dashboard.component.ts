import { DashboardItemComponent } from './dashboard-item/dashboard-item.component';
import { Component, OnInit, ViewChild, ViewChildren, QueryList, ComponentFactoryResolver, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NbDialogService, NbMenuService } from '@nebular/theme';
import * as moment from 'moment';
import { Observable, of, forkJoin } from 'rxjs';
import { map, catchError, filter } from 'rxjs/operators';
import { GridComponent } from './grid/grid.component';
import { LineChartWidgetComponent } from '../widgets/line-chart-widget/line-chart-widget.component';
import { TimePeriod } from '@app/core/shared/models/timeperiod.model';
import { DatetimeRange } from '../shared/daterange-picker/datetime-range.model';
import { ArrayExtension } from '@app/core/shared/utils/groupby';
import { WidgetsOverviewComponent } from './widgets-overview/widgets-overview.component';
import { GuidGenerator } from '@app/core/shared/utils/guid-generator';
import { Item } from './grid-item/grid-item.component';
import { WidgetBuilderComponent } from './../widget-builder/widget-builder.component';
import { WidgetBuilderService } from './../widget-builder/widget-builder.service';
import { DashboardDto, DashboardClient, WidgetClient, DashboardWidget, UpdateDashboardCommand, WidgetDto } from '@app/web-api-client';

export class WidgetContextMenuData {
  public widgetId: string;
  public option: 'edit' | 'delete';
}

export class DashboardGridWidget {
  public widgetId: string;
  public widgetType: string;
  public widgetName: string;
  public dashboardWidgetId: string | null;
  public gridItem: Item;
  public widgetContextMenu: any;
  public isDashboardWidgetNew: boolean;
  public widgetData: any;
  public requiresDataLoading: boolean;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {

  @ViewChild(GridComponent, { static: true }) stackgrid: GridComponent;
  @ViewChildren('dashboarditem') components: QueryList<DashboardItemComponent>;

  requiresSaving = false;
  loading = true;
  items: DashboardGridWidget[] = [];
  dashboard: DashboardDto;
  timer: any;
  savingDashboard = false;

  selectedDaterange: DatetimeRange = { start: moment().subtract(12, 'hours'), end: null /* null for NOW */ };
  selectedTimePeriod: TimePeriod = { start: this.selectedDaterange?.start?.unix(), end: this.selectedDaterange?.end?.unix() };

  constructor(private dashboardClient: DashboardClient, private widgetClient: WidgetClient,
    private dialogService: NbDialogService, private route: ActivatedRoute, private nbMenuService: NbMenuService,
    private widgetService: WidgetBuilderService, private cdr: ChangeDetectorRef) {
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.loading = true;
      this.items = [];
      this.dashboard = null;
      this.requiresSaving = false;
      this.cdr.detectChanges();

      const dashboardId = params.get('dashboardId');
      this.dashboardClient.get(dashboardId).subscribe(dashboardDetails => {
        this.dashboard = dashboardDetails.dashboard;

        dashboardDetails.dashboardWidgets.forEach(dashboardWidget => {
          const item = new DashboardGridWidget;
          item.dashboardWidgetId = dashboardWidget.id;
          item.isDashboardWidgetNew = false;
          item.requiresDataLoading = true;
          item.widgetId = dashboardWidget.widgetId;
          item.widgetType = dashboardWidget.widgetType;
          // item.widgetContextMenu = this.getWidgetContextMenu(item.dashboardWidgetId);
          item.gridItem = {
            el: null,
            autoPosition: false,
            height: dashboardWidget.height,
            width: dashboardWidget.width,
            id: GuidGenerator.newGuid(),
            x: dashboardWidget.x,
            y: dashboardWidget.y
          };

          this.items.push(item);
        });

        if (dashboardDetails.dashboardWidgets.length === 0) {
          this.loading = false;
        }

        this.loadDashboardWidgetsData();
      });
    });

    this.nbMenuService.onItemClick()
      .pipe(
        filter(({ tag }) => tag === 'widget-context-menu'),
        map(({ item: { data } }) => data)
      )
      .subscribe((data: WidgetContextMenuData) => {
        switch (data.option) {
          case 'delete':
            this.deleteWidget(data.widgetId);
            break;
          case 'edit':
            this.editWidget(data.widgetId);
            break;
        }
      });

    this.timer = setInterval(() => {
      this.components.forEach(x => {
        x?.loadData();
      });
    }, 2000);
  }

  loadDashboardWidgetsData() {
    const dashboardWidgetsToLoad = this.items.filter(x => x.requiresDataLoading);

    if (!dashboardWidgetsToLoad || dashboardWidgetsToLoad.length === 0) {
      return;
    }

    // Group widgets by type
    const groupedWidgets = ArrayExtension.groupBy(dashboardWidgetsToLoad, x => x.widgetType);
    this.loading = true;

    const widgetsObservables: Observable<{ widget: WidgetDto, widgetData: any }>[] = groupedWidgets.map((group, groupIndex) => {
      return this.widgetService.getMultipleWidgetDataByType(group.key, group.members.map(x => x.widgetId))
        .pipe(
          map(widgets => {
            //this.processes[groupIndex].tasks = tasks; // assign tasks to each process as they arrive
            return widgets;
          })
          , catchError((error: any) => {
            console.error('Error loading widgets for widget: ' + group, 'Error: ', error);
            return of(null); // In case error occurs, we need to return Observable, so the stream can continue
          })
        );
    });

    forkJoin(widgetsObservables).subscribe(
      widgetsArray => {
        // In case error occurred e.g. for the process at position 1,
        // Output will be: [[Task], null, [Task]];
        // flatten widgetsArray
        const widgets: { widget: WidgetDto, widgetData: any }[] =
          widgetsArray.reduce((accumulator, value) => accumulator.concat(value), []);

        dashboardWidgetsToLoad.forEach(dashboardWidget => {
          const widgetResult = widgets.find(x => x.widget.id === dashboardWidget.widgetId);
          dashboardWidget.widgetData = widgetResult.widgetData;
          dashboardWidget.widgetName = widgetResult.widget.name;
          dashboardWidget.requiresDataLoading = false;
        });

        this.loading = false;
      }
    );
  }


  initWidgets() {
    //this.components.changes.subscribe((r: DashboardItemComponent) => {
    //  console.log('count: ' + this.components.length);

    //  this.components.forEach(x => {
    //    const factory = this.componentFactoryResolver.resolveComponentFactory(LineChartWidgetComponent);

    //    var widgetComponent = x.createComponent(factory)
    //    widgetComponent.instance.lineChartWidget = this.lineChartWidget
    //    widgetComponent.instance.data = this.lineChartData
    //  }
    //  )
    //});
  }

  saveDashboard() {
    this.savingDashboard = true;

    const finalDashboardWidgets: DashboardWidget[] = this.items.map(dashboardWidget => DashboardWidget.fromJS({
      widgetId: dashboardWidget.widgetId,
      id: dashboardWidget.dashboardWidgetId,
      width: dashboardWidget.gridItem.width,
      height: dashboardWidget.gridItem.height,
      x: dashboardWidget.gridItem.x,
      y: dashboardWidget.gridItem.y,
      dashboardId: this.dashboard.id
    }));

    const updateDashboardCommand = new UpdateDashboardCommand;
    updateDashboardCommand.applicationId = this.dashboard.applicationId;
    updateDashboardCommand.dashboardId = this.dashboard.id;
    updateDashboardCommand.dashboardWidgets = finalDashboardWidgets;

    this.dashboardClient.update(updateDashboardCommand).subscribe(() => {
      this.savingDashboard = false;
      this.requiresSaving = false;
    });
  }


  handleGridstackChangeEvent($event) {
    console.log('grid stack changed');
    this.requiresSaving = true;
  }

  ngOnDestroy() {
    clearInterval(this.timer);
  }

  handleSelectedDaterangeChange(newDateRange: DatetimeRange) {
    const date30SecondsAgo = moment().subtract(30, 'seconds');

    // If the end date is within the last 30 seconds, set it to NOW
    if (newDateRange.end && newDateRange.end.isBetween(date30SecondsAgo, moment())) {
      newDateRange.end = null;
    }

    this.selectedDaterange = newDateRange;
    this.selectedTimePeriod = { start: this.selectedDaterange?.start?.unix(), end: this.selectedDaterange?.end?.unix() };
  }

  addWidget() {
    this.dialogService.open(WidgetsOverviewComponent, {
      context: {
        applicationId: this.dashboard.applicationId,
      },
    }).onClose.subscribe((newAddedWidgets: WidgetDto[]) => {
      if (newAddedWidgets && newAddedWidgets.length > 0) {
        newAddedWidgets.forEach(newWidget => {
          const item = new DashboardGridWidget;
          item.dashboardWidgetId = GuidGenerator.newGuid();
          item.isDashboardWidgetNew = true;
          item.requiresDataLoading = true;
          item.widgetId = newWidget.id;
          item.widgetType = newWidget.type;
          // item.widgetContextMenu = this.getWidgetContextMenu(item.dashboardWidgetId);
          item.gridItem = {
            el: null,
            autoPosition: false,
            height: 2,
            width: 4,
            id: GuidGenerator.newGuid(),
            x: 0,
            y: 0
          };

          this.items.push(item);
        });
        this.loadDashboardWidgetsData();
        this.requiresSaving = true;
      }
    });
  }

  deleteWidget(dashboardWidgetId: string): void {
    const itemIndex = this.items.findIndex(x => x.dashboardWidgetId === dashboardWidgetId);
    if (itemIndex > -1) {
      this.items.splice(itemIndex, 1);
      this.requiresSaving = true;
    }
  }

  editWidget(dashboardWidgetId: string): void {
    const dashboardGridWidget = this.items.find(x => x.dashboardWidgetId === dashboardWidgetId);
    console.log('edit: ' + dashboardWidgetId);
    console.log(dashboardGridWidget);
    if (dashboardGridWidget !== null && dashboardGridWidget !== undefined) {
      this.dialogService.open(WidgetBuilderComponent, {
        context: {
          widgetId: dashboardGridWidget.widgetId
        },
      }).onClose.subscribe((thereAreChanges: boolean) => {
        dashboardGridWidget.requiresDataLoading = true;
        this.loadDashboardWidgetsData();
        // const component = this.components.find(x => x.item.dashboardWidgetId === dashboardWidgetId);
        // component.reloadWidget();
      });
    }
  }
}
