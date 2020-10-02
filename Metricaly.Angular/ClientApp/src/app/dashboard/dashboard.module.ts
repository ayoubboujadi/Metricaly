import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// My components
import { DashboardComponent } from './dashboard.component';
import { RouterModule } from '@angular/router';
import { GridItemComponent } from './grid-item/grid-item.component';
import { GridComponent } from './grid/grid.component';
import { ChartsModule } from 'ng2-charts';
import { WidgetBuilderComponent } from './widget-builder/widget-builder.component';
import { EditableFieldComponent } from './editable-field/editable-field.component';
import { WidgetSettingsEditorComponent } from './widget-settings-editor/widget-settings-editor.component';

// Others
import { ColorSketchModule } from 'ngx-color/sketch';
import { ColorSliderModule } from 'ngx-color/slider'; // <color-slider></color-slider>
import { ColorPickerDialogComponent } from './color-picker-dialog/color-picker-dialog.component';
import { AutofocusDirective } from './editable-field/autofocus.directive';
import { ColorPickerModule } from '@iplab/ngx-color-picker';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// Nebular
import { NbThemeModule, NbCardModule, NbTabsetModule, NbListModule, NbButtonModule, NbIconModule, NbDialogModule } from '@nebular/theme';

@NgModule({
  declarations: [DashboardComponent, GridItemComponent, GridComponent, WidgetBuilderComponent, EditableFieldComponent, AutofocusDirective, WidgetSettingsEditorComponent, ColorPickerDialogComponent],
  imports: [
    CommonModule,
    ChartsModule,
    FormsModule,
    ReactiveFormsModule,

    NbThemeModule,
    NbCardModule,
    NbTabsetModule,
    NbListModule,
    NbButtonModule,
    NbIconModule,
    NbDialogModule.forChild(),

    ColorSketchModule,

    RouterModule.forChild([
      {
        path: '',
        component: DashboardComponent
      },
      {
        path: 'builder',
        component: WidgetBuilderComponent
      }
    ])
  ]
})
export class DashboardModule { }
