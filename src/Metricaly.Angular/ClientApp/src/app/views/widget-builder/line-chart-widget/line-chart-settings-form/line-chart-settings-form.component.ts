import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { LineChartWidgetSettings, LineChartAxisSettings } from '@app/web-api-client';

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
    connectNulls: [null],
    xAxis: this.fb.group({
      label: [''],
    }),
    yAxisLeft: this.fb.group({
      label: [''],
      min: [null],
      minOption: [null],
      max: [null],
      maxOption: [null],
    }),
    yAxisRight: this.fb.group({
      label: [null],
      min: [null],
      minOption: [null],
      max: [null],
      maxOption: [null],
    }),
  });


  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.widgetSettingsForm.valueChanges.subscribe(val => {
      this.submitButtonDisabled = false;
      this.onSubmit();
    });
  }

  get f() { return this.widgetSettingsForm.controls; }

  parseAxisSettings(groupName: string): LineChartAxisSettings {
    const axisSettings = LineChartAxisSettings.fromJS({
      label: this.widgetSettingsForm.get([groupName, 'label']).value,
    });

    const minSelectedOption = this.widgetSettingsForm.get([groupName, 'minOption']).value;
    if (minSelectedOption === 'data') {
      axisSettings.isMinData = true;
    } else if (minSelectedOption === 'custom') {
      axisSettings.min = this.widgetSettingsForm.get([groupName, 'min']).value ?? 0;
    }

    const maxSelectedOption = this.widgetSettingsForm.get([groupName, 'maxOption']).value;
    if (maxSelectedOption === 'data') {
      axisSettings.isMaxData = true;
    } else if (maxSelectedOption === 'custom') {
      axisSettings.max = this.widgetSettingsForm.get([groupName, 'max']).value ?? 0;
    }

    return axisSettings;
  }

  onSubmit() {
    const settings = LineChartWidgetSettings.fromJS({
      displayLegend: this.widgetSettingsForm.get('displayLegend').value,
      smoothLines: this.widgetSettingsForm.get('smoothLines').value,
      filled: this.widgetSettingsForm.get('filled').value,
      connectNulls: this.widgetSettingsForm.get('connectNulls').value,
      xAxisSettings: {
        label: this.widgetSettingsForm.get(['xAxis', 'label']).value,
      },
      yLeftAxisSettings: this.parseAxisSettings('yAxisLeft'),
      yRightAxisSettings: this.parseAxisSettings('yAxisRight')
    });
    this.settingsChange.emit(settings);
    this.submitButtonDisabled = true;
  }

  updateFormValues() {
    const yLeftMinOption = this.getSelectedOption(this.settings.yLeftAxisSettings.min, this.settings.yLeftAxisSettings.isMinData);
    this.selectedAxisMinMaxChanged('yAxisLeft', 'min', yLeftMinOption);

    const yLeftMaxOption = this.getSelectedOption(this.settings.yLeftAxisSettings.max, this.settings.yLeftAxisSettings.isMaxData);
    this.selectedAxisMinMaxChanged('yAxisLeft', 'max', yLeftMaxOption);

    const yRightMinOption = this.getSelectedOption(this.settings.yRightAxisSettings.min, this.settings.yRightAxisSettings.isMinData);
    this.selectedAxisMinMaxChanged('yAxisRight', 'min', yRightMinOption);

    const yRightMaxOption = this.getSelectedOption(this.settings.yRightAxisSettings.max, this.settings.yRightAxisSettings.isMaxData);
    this.selectedAxisMinMaxChanged('yAxisRight', 'max', yRightMaxOption);

    this.widgetSettingsForm.patchValue({
      displayLegend: this.settings.displayLegend,
      smoothLines: this.settings.smoothLines,
      filled: this.settings.filled,
      connectNulls: this.settings.connectNulls,
      xAxis: {
        label: this.settings.xAxisSettings.label
      },
      yAxisLeft: {
        label: this.settings.yLeftAxisSettings.label,
        min: this.settings.yLeftAxisSettings.min,
        max: this.settings.yLeftAxisSettings.max,
        minOption: yLeftMinOption,
        maxOption: yLeftMaxOption,
      },
      yAxisRight: {
        label: this.settings.yRightAxisSettings.label,
        min: this.settings.yRightAxisSettings.min,
        max: this.settings.yRightAxisSettings.max,
        minOption: yRightMinOption,
        maxOption: yRightMaxOption,
      },
    }, { emitEvent: false });
  }

  resetFormValues() {
    this.submitButtonDisabled = true;
    this.updateFormValues();
  }

  getSelectedOption(value: number, isData: boolean): string {
    if (isData) {
      return 'data';
    } else if (value === null || value === undefined) {
      return 'dynamic';
    } else {
      return 'custom';
    }
  }

  selectedAxisMinMaxChanged(groupName: string, fieldName: string, selectedValue: string) {
    if (selectedValue === 'custom') {
      this.widgetSettingsForm.get([groupName, fieldName]).enable({ emitEvent: false });
    } else {
      this.widgetSettingsForm.get([groupName, fieldName]).disable({ emitEvent: false });
    }
  }

}
