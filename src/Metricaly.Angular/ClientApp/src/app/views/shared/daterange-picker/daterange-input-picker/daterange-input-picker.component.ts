import { Component, OnInit } from '@angular/core';
import { DaterangepickerComponent } from 'ngx-daterangepicker-material';
import * as moment from 'moment';

@Component({
  selector: 'app-daterange-input-picker',
  templateUrl: './daterange-input-picker.component.html',
  styleUrls: ['./daterange-input-picker.component.scss']
})
export class DaterangeInputPickerComponent extends DaterangepickerComponent {


  timeButtons: any = [
    { value: 1, unit: 'm', label: '5 min', selected: false},
    { value: 15, unit: 'm', label: '15 min', selected: false},
    { value: 30, unit: 'm', label: '30 min', selected: false},
    { value: 1, unit: 'h', label: '1 hour', selected: false},
    { value: 3, unit: 'h', label: '3 hour', selected: false },
    { value: 12, unit: 'h', label: '12 hour', selected: false },
  ];


  handleTimeClick(item: any) {
    item.selected = true;
  }
}
