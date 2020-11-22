import { WidgetBuilderService } from './widget-builder.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import { DatetimeRange } from '@app/views/shared/daterange-picker/datetime-range.model';
import { SubHeaderService } from '@app/views/shared/sub-header/sub-header.service';
import { WidgetDto, WidgetClient } from '@app/web-api-client';
import { mergeMap } from 'rxjs/operators';

@Component({
  selector: 'app-widget-builder',
  templateUrl: './widget-builder.component.html',
  styleUrls: ['./widget-builder.component.css']
})
export class WidgetBuilderComponent implements OnInit, OnDestroy {

  selectedDaterange: DatetimeRange = { start: moment().subtract(1, 'hours'), end: null /* null for NOW */ };
  widgetData: any;
  widget: WidgetDto;
  applicationId: string;
  loading = true;
  savingWidget = false;

  constructor(private route: ActivatedRoute, private widgetClient: WidgetClient, private subHeaderService: SubHeaderService,
    private widgetBuilderService: WidgetBuilderService) {
  }

  ngOnInit(): void {
    // Get the widget data from the API
    this.route.paramMap.subscribe(params => {
      const widgetId = params.get('widgetId');

      this.widgetClient.getWidget(widgetId)
        .pipe(
          mergeMap(widget => {
            this.applicationId = widget.applicationId;
            this.widget = widget;
            this.subHeaderService.setTitle(widget.name);
            return this.widgetBuilderService.getWidgetDataByType(widget.type, widgetId);
          })
        ).subscribe(
          (result) => {
            this.widgetData = result.widgetData;
            this.loading = false;
          },
          (error) => {
            console.error(error);
          }
        );

      /*this.widgetClient.getWidget(widgetId)
        .subscribe(
          (result) => {
            this.applicationId = result.applicationId;
            this.widget = result;
            this.subHeaderService.setTitle(result.name);
            this.widgetBuilderService.getWidgetDataByType(result.type, widgetId).subscribe(
              (result2) => {
                this.widgetData = result2.widgetData;
              },
              (error) => {
                console.error(error);
              }
            );
            this.loading = false;
          }
        );*/
    });
  }

  handleSelectedDaterangeChange(newDateRange: DatetimeRange) {
    const date30SecondsAgo = moment().subtract(30, 'seconds');

    // If the end date is within the last 30 seconds, set it to NOW
    if (newDateRange.end && newDateRange.end.isBetween(date30SecondsAgo, moment())) {
      newDateRange.end = null;
    }

    this.selectedDaterange = newDateRange;
  }


  saveWidget() {
    this.savingWidget = true;
    // Update the widget
    this.widgetBuilderService.saveWidgetData(this.widget.type, this.widget.id, this.widget.name, this.widgetData)
      .subscribe(() => this.savingWidget = false);
  }

  ngOnDestroy(): void {
    this.subHeaderService.setTitle(null);
  }

}
