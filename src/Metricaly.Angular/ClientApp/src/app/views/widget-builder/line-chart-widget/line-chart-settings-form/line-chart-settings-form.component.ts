import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { LineChartWidgetSettings } from '@app/web-api-client';

@Component({
  selector: 'app-line-chart-settings-form',
  templateUrl: './line-chart-settings-form.component.html',
  styleUrls: ['./line-chart-settings-form.component.css']
})
export class LineChartSettingsFormComponent implements OnInit {

  private _settings: LineChartWidgetSettings;
  @Output() settingsChange: EventEmitter<LineChartWidgetSettings> = new EventEmitter;

  @Input() set settings(settings: LineChartWidgetSettings) {
    this._settings = settings;
    this.updateFormValues();
  }

  get settings(): LineChartWidgetSettings {
    return this._settings;
  }

  submitButtonDisabled = true;

  widgetSettingsForm = this.fb.group({
    displayLegend: [null],
    smoothLines: [null],
    filled: [null],
    xAxis: this.fb.group({
      label: [''],
    }),
    yAxisLeft: this.fb.group({
      label: [''],
    }),
    yAxisRight: this.fb.group({
      label: [''],
    }),
  });


  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.widgetSettingsForm.valueChanges.subscribe(val => {
      this.submitButtonDisabled = false;
      this.onSubmit();
    });
  }

  onSubmit() {

    const settings = LineChartWidgetSettings.fromJS({
      displayLegend: this.widgetSettingsForm.get('displayLegend').value,
      smoothLines: this.widgetSettingsForm.get('smoothLines').value,
      filled: this.widgetSettingsForm.get('filled').value,
      xAxisSettings: {
        label: this.widgetSettingsForm.get(['xAxis', 'label']).value,
      },
      yLeftAxisSettings: {
        label: this.widgetSettingsForm.get(['yAxisLeft', 'label']).value
      },
      yRightAxisSettings: {
        label: this.widgetSettingsForm.get(['yAxisRight', 'label']).value
      }
    });

    this.settingsChange.emit(settings);
    this.submitButtonDisabled = true;
  }

  updateFormValues() {
    this.widgetSettingsForm.patchValue({
      displayLegend: this.settings.displayLegend,
      smoothLines: this.settings.smoothLines,
      filled: this.settings.filled,
      xAxis: {
        label: this.settings.xAxisSettings.label
      },
      yAxisLeft: {
        label: this.settings.yLeftAxisSettings.label,
      },
      yAxisRight: {
        label: this.settings.yRightAxisSettings.label,
      },
    }, { emitEvent: false });
  }

  resetFormValues() {
    this.submitButtonDisabled = true;
    this.updateFormValues();
  }

}
