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
    widgetName: [''],
    displayTitle: [null],
    displayLegend: [null],
    smoothLines: [null],
    stacked: [null],
    xAxis: this.fb.group({
      label: [''],
      displayLabel: [null],
      displayGridLines: [null]
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
      title: this.widgetSettingsForm.get('widgetName').value,
      displayTitle: this.widgetSettingsForm.get('displayTitle').value,
      displayLegend: this.widgetSettingsForm.get('displayLegend').value,
      smoothLines: this.widgetSettingsForm.get('smoothLines').value,
      stacked: this.widgetSettingsForm.get('stacked').value,
      xAxisSettings: {
        label: this.widgetSettingsForm.get(['xAxis', 'label']).value,
        displayLabel: this.widgetSettingsForm.get(['xAxis', 'displayLabel']).value,
        displayGridLines: this.widgetSettingsForm.get(['xAxis', 'displayGridLines']).value,
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
      widgetName: this.settings.title,
      displayTitle: this.settings.title,
      displayLegend: this.settings.displayLegend,
      smoothLines: this.settings.smoothLines,
      stacked: this.settings.filled,
      xAxis: {
        label: this.settings.xAxisSettings.label,
        displayLabel: this.settings.xAxisSettings.displayLabel,
        displayGridLines: this.settings.xAxisSettings.displayGridLines
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
    this.updateFormValues()
  }

}
