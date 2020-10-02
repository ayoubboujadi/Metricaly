import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Validators } from '@angular/forms';
import { FormArray } from '@angular/forms';
import { WidgetSettings } from '../models/widget-settings.model';

@Component({
  selector: 'app-widget-settings-editor',
  templateUrl: './widget-settings-editor.component.html',
  styleUrls: ['./widget-settings-editor.component.css']
})
export class WidgetSettingsEditorComponent implements OnInit {

  private _settings: WidgetSettings
  @Output() settingsChange: EventEmitter<WidgetSettings> = new EventEmitter

  @Input() set settings(settings: WidgetSettings) {
    this._settings = settings;
    console.log("set was called on widgetSettingsEditor")
    this.updateFormValues()
  }

  get settings(): WidgetSettings {
    return this._settings
  }

  submitButtonDisabled: boolean = true

  widgetSettingsForm = this.fb.group({
    widgetName: [''],
    displayTitle: [null],
    xAxis: this.fb.group({
      label: [''],
      displayLabel: [null],
      displayGridLines: [null]
    }),
  });


  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.widgetSettingsForm.valueChanges.subscribe(val => {
      this.submitButtonDisabled = false
      this.onSubmit() 
    });
  }

  onSubmit() {
    console.log("Widget settings form onSubmit()")

    var settings = new WidgetSettings
    settings.title = this.widgetSettingsForm.get('widgetName').value;
    settings.displayTitle = this.widgetSettingsForm.get('displayTitle').value

    settings.xAxisSettings.label = this.widgetSettingsForm.get(['xAxis', 'label']).value;
    settings.xAxisSettings.displayLabel = this.widgetSettingsForm.get(['xAxis', 'displayLabel']).value;
    settings.xAxisSettings.displayGridLines = this.widgetSettingsForm.get(['xAxis', 'displayGridLines']).value;


    this.settingsChange.emit(settings)

    this.submitButtonDisabled = true
  }

  updateFormValues() {
    this.widgetSettingsForm.patchValue({
      widgetName: this.settings.title,
      displayTitle: this.settings.displayTitle,
      xAxis: {
        label: this.settings.xAxisSettings.label,
        displayLabel: this.settings.xAxisSettings.displayLabel,
        displayGridLines: this.settings.xAxisSettings.displayGridLines,
      }
    }, { emitEvent: false });
  }

  resetFormValues() {
    this.submitButtonDisabled = true
    this.updateFormValues()
  }

}
