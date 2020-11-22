import { Component, Inject, OnInit, Input, Output, EventEmitter, HostBinding, HostListener } from '@angular/core';
import { ColorEvent } from 'ngx-color';
import { NbDialogRef } from '@nebular/theme';
import { ColorPickerControl, Color } from '@iplab/ngx-color-picker';

@Component({
  selector: 'app-color-picker-dialog',
  templateUrl: './color-picker-dialog.component.html',
  styleUrls: ['./color-picker-dialog.component.css']
})
export class ColorPickerDialogComponent implements OnInit {

  public color: string
  constructor(
    protected dialogRef: NbDialogRef<ColorPickerDialogComponent>
  ) { }

  ngOnInit(): void {
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }

  changeComplete(event: ColorEvent) {
    this.color = event.color.hex
  }

  submit() {
    this.dialogRef.close(this.color);
  }

}
