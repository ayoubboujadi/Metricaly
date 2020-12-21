import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { DatetimeRange } from './datetime-range.model';
import * as moment from 'moment';
import { DurationInputArg2 } from 'moment';

@Component({
  selector: 'app-daterange-picker',
  templateUrl: './daterange-picker.component.html',
  styleUrls: ['./daterange-picker.component.css']
})
export class DaterangePickerComponent implements OnInit {

  @Input()
  selectedDaterange: DatetimeRange;

  @Output()
  selectedDaterangeChange: EventEmitter<DatetimeRange> = new EventEmitter();

  ranges: any = {
    'Today': [moment().startOf('day'), moment()],
    'Yesterday': [moment().subtract(1, 'days').startOf('day'), moment().subtract(1, 'days').endOf('day')],
    'Last 7 Days': [moment().subtract(6, 'days').startOf('day'), moment().endOf('day')],
    'Last 30 Days': [moment().subtract(29, 'days').startOf('day'), moment().endOf('day')],
    'This Month': [moment().startOf('month').startOf('day'), moment().endOf('month').endOf('day')],
    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month').endOf('day')]
  };

  timeButtons: any = [
    { value: 1, unit: 'm', rawValue: '1m', label: '1 min', selected: false },
    { value: 5, unit: 'm', rawValue: '5m', label: '5 min', selected: false },
    { value: 15, unit: 'm', rawValue: '15m', label: '15 min', selected: false },
    { value: 30, unit: 'm', rawValue: '30m', label: '30 min', selected: false },
    { value: 1, unit: 'h', rawValue: '1h', label: '1 hour', selected: false },
    { value: 3, unit: 'h', rawValue: '3h', label: '3 hours', selected: false },
    { value: 6, unit: 'h', rawValue: '6h', label: '6 hours', selected: false },
    { value: 12, unit: 'h', rawValue: '12h', label: '12 hours', selected: false },
    { value: 24, unit: 'h', rawValue: '1d', label: '1 day', selected: false },
  ];

  ngOnInit(): void {
    this.selectedDaterange = { start: null, end: null, liveSpan: '1h' };
  }

  handleSelectedDaterangeChange(newDateRange: DatetimeRange) {
    const date30SecondsAgo = moment().subtract(30, 'seconds');

    // If the end date is within the last 30 seconds, set it to NOW
    if (newDateRange.end && newDateRange.end.isBetween(date30SecondsAgo, moment())) {
      newDateRange.end = null;
    }

    this.selectedDaterange = newDateRange;
    this.selectedDaterangeChange.emit(this.selectedDaterange);
  }

  handleTimeClick(item: any) {
    this.timeButtons.forEach(x => x.selected = false);
    item.selected = true;
    const newDateRange = new DatetimeRange;
    newDateRange.start = moment().subtract(item.value, item.unit as DurationInputArg2);
    newDateRange.liveSpan = item.rawValue;

    this.selectedDaterange = newDateRange;
    this.selectedDaterangeChange.emit(this.selectedDaterange);
  }
}
