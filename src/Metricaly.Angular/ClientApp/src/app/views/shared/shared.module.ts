import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { EditableFieldComponent } from './editable-field/editable-field.component';
import { ColorPickerDialogComponent } from './color-picker-dialog/color-picker-dialog.component';
import { AlertComponent } from './alert/alert.component';
import { AutofocusDirective } from './editable-field/autofocus.directive';
import { ColorSketchModule } from 'ngx-color/sketch';
import { NbButtonModule, NbCardModule, NbInputModule, NbIconModule, NbFormFieldModule, NbSelectModule } from '@nebular/theme';
import { DaterangePickerComponent } from './daterange-picker/daterange-picker.component';
import { NgxDaterangepickerMd } from 'ngx-daterangepicker-material';
import { DaterangepickerDirective } from './daterange-picker/daterange-input-picker/daterangepicker.directive';
import { DaterangeInputPickerComponent } from './daterange-picker/daterange-input-picker/daterange-input-picker.component';
import { ConfirmDialogComponent } from './dialogs/confirm-dialog/confirm-dialog.component';
import { BreadcrumbComponent } from './breadcrumb/breadcrumb.component';
import { RouterModule } from '@angular/router';
import { SubHeaderComponent } from './sub-header/sub-header.component';

@NgModule({
  declarations: [
    EditableFieldComponent,
    ColorPickerDialogComponent,
    AlertComponent,
    AutofocusDirective,
    DaterangepickerDirective,
    DaterangePickerComponent,
    DaterangeInputPickerComponent,
    ConfirmDialogComponent,
    BreadcrumbComponent,
    SubHeaderComponent,
  ],
  imports: [
    CommonModule,
    NbCardModule,
    NbButtonModule,
    ColorSketchModule,
    FormsModule,
    NbInputModule,
    NbIconModule,
    NbFormFieldModule,
    NbSelectModule,
    NgxDaterangepickerMd,
    RouterModule
  ],
  exports: [
    EditableFieldComponent,
    ColorPickerDialogComponent,
    AlertComponent,
    DaterangePickerComponent,
    DaterangepickerDirective,
    BreadcrumbComponent,
    SubHeaderComponent,
    ConfirmDialogComponent
  ]
})
export class SharedModule { }
