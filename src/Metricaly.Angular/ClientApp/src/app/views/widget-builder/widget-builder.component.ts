import { WidgetBuilderService } from './widget-builder.service';
import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import { DatetimeRange } from '@app/views/shared/daterange-picker/datetime-range.model';
import { SubHeaderService } from '@app/views/shared/sub-header/sub-header.service';
import { WidgetDto, WidgetClient } from '@app/web-api-client';
import { mergeMap } from 'rxjs/operators';
import { NbToastrService, NbGlobalPhysicalPosition } from '@nebular/theme';

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

  @Input() widgetId: string;

  constructor(private route: ActivatedRoute, private widgetClient: WidgetClient, private subHeaderService: SubHeaderService,
    private widgetBuilderService: WidgetBuilderService, private toastrService: NbToastrService) {
  }

  ngOnInit(): void {
    // Get the widget data from the API
    this.route.paramMap.subscribe(params => {

      if (this.widgetId === null || this.widgetId === undefined) {
        this.widgetId = params.get('widgetId');
      }

      this.widgetClient.getWidget(this.widgetId)
        .pipe(
          mergeMap(widget => {
            this.applicationId = widget.applicationId;
            this.widget = widget;
            this.subHeaderService.setTitle(widget.name);
            return this.widgetBuilderService.getWidgetDataByType(widget.type, this.widgetId);
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
      .subscribe(() => {
        this.savingWidget = false;
        this.toastrService.show('Success', 'Widget was saved successfully!',
          { position: NbGlobalPhysicalPosition.TOP_RIGHT, status: 'success' });
      });
  }

  ngOnDestroy(): void {
    this.subHeaderService.setTitle(null);
  }

}
