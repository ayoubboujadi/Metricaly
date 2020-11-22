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
    { value: 1, unit: 'm', label: '5 min', selected: false},
    { value: 15, unit: 'm', label: '15 min', selected: false},
    { value: 30, unit: 'm', label: '30 min', selected: false},
    { value: 1, unit: 'h', label: '1 hour', selected: false},
    { value: 3, unit: 'h', label: '3 hour', selected: false },
    { value: 12, unit: 'h', label: '12 hour', selected: false },
  ];

  ngOnInit(): void {
    this.selectedDaterange = { start: moment().subtract(1, 'hours'), end: null /* null for NOW */ };
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

    this.selectedDaterange = newDateRange;
    this.selectedDaterangeChange.emit(this.selectedDaterange);
  }
}
