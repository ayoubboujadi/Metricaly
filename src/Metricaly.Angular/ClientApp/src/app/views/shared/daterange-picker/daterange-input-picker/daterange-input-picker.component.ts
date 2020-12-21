import { Component, OnInit } from '@angular/core';
import { DaterangepickerComponent } from 'ngx-daterangepicker-material';
import * as moment from 'moment';

@Component({
  selector: 'app-daterange-input-picker',
  templateUrl: './daterange-input-picker.component.html',
  styleUrls: ['./daterange-input-picker.component.scss']
})
export class DaterangeInputPickerComponent extends DaterangepickerComponent {

}
