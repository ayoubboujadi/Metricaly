import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { NbToastrService, NbDialogRef } from '@nebular/theme';
import { WidgetClient, WidgetDto } from '@app/web-api-client';

@Component({
  selector: 'app-widgets-overview',
  templateUrl: './widgets-overview.component.html',
  styleUrls: ['./widgets-overview.component.css']
})
export class WidgetsOverviewComponent implements OnInit {

  @Input() applicationId: string;
  @Input() existingWidgetsIds: string[] = [];
  @Output() widgetAddedEvent: EventEmitter<WidgetDto> = new EventEmitter;

  loading = true;
  widgets: WidgetDto[] = [];
  addedWidgets: WidgetDto[] = [];

  constructor(private widgetClient: WidgetClient, private toastrService: NbToastrService,
    protected dialogRef: NbDialogRef<WidgetsOverviewComponent>) {
  }

  ngOnInit(): void {
    this.widgetClient.list(this.applicationId).subscribe((result) => {
      this.widgets = result;
      this.loading = false;
    });
  }

  addWidget(widget: WidgetDto): void {
    this.existingWidgetsIds.push(widget.id);
    this.widgetAddedEvent.emit(widget);
    this.addedWidgets.push(widget);
    this.showWidgetAddedToast(widget.name);
  }

  showWidgetAddedToast(widgetName: string): void {
    this.toastrService.show('', widgetName + ' was added to the dashboard!', { status: 'success' });
  }

  save() {
    this.dialogRef.close(this.addedWidgets);
  }

  close() {
    this.dialogRef.close();
  }
}
