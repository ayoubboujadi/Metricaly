import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";

@Component({
  selector: 'app-editable-field',
  templateUrl: './editable-field.component.html',
  styleUrls: ['./editable-field.component.css']
})
export class EditableFieldComponent<T> implements OnInit {
  _data: T;

  @Input() set data(value: T) {
    this._data = value;
    if (this.originalData == null) {
      this.originalData = value;
    }
    console.log("data setter : " + value);
  }

  get data(): T {
    return this._data;
  }

  @Output() focusOut: EventEmitter<T> = new EventEmitter<T>();
  @Input() inputType: string = "text"

  editMode = false;
  originalData: T;

  constructor() { }

  ngOnInit() { }

  save(event) {
    this.originalData = this.data;
    this.focusOut.emit(this.data);
    event.preventDefault();
    this.editMode = false;
  }

  cancel() {
    this.data = this.originalData;
    this.editMode = false;
  }

}
