import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import {
    LineChartWidgetClient,
    SimpleNumberWidgetClient,
    UpdateWidgetCommandOfLineChartWidget,
    WidgetDto,
    UpdateWidgetCommandOfSimpleNumberWidget
} from '@app/web-api-client';

@Injectable({
    providedIn: 'root'
})
export class WidgetBuilderService {

    constructor(private lineChartWidgetClient: LineChartWidgetClient, private simpleNumberWidgetClient: SimpleNumberWidgetClient) { }

    getWidgetDataByType(widgetType: string, widgetId: string): Observable<{ widget: WidgetDto, widgetData: any }> {
        if (widgetType === 'LineChart') {
            return this.lineChartWidgetClient.get(widgetId)
                .pipe(
                    map(item => ({ widget: item.widget, widgetData: item.widgetData as any }))
                );
        } else if (widgetType === 'SimpleNumber') {
            return this.simpleNumberWidgetClient.get(widgetId)
                .pipe(
                    map(item => ({ widget: item.widget, widgetData: item.widgetData as any }))
                );
        }
    }

    getMultipleWidgetDataByType(widgetType: string, widgetIds: string[]): Observable<{ widget: WidgetDto, widgetData: any }[]> {
        if (widgetType === 'LineChart') {
            return this.lineChartWidgetClient.readMultiple(widgetIds)
                .pipe(
                    map(items => items.map(item => ({ widget: item.widget, widgetData: item.widgetData as any })))
                );
        } else if (widgetType === 'SimpleNumber') {
            return this.simpleNumberWidgetClient.readMultiple(widgetIds)
                .pipe(
                    map(items => items.map(item => ({ widget: item.widget, widgetData: item.widgetData as any })))
                );
        }
    }

    saveWidgetData(widgetType: string, widgetId: string, widgetName: string, widgetData: any): any {
        if (widgetType === 'LineChart') {
            const data = UpdateWidgetCommandOfLineChartWidget.fromJS({ id: widgetId, name: widgetName, widgetData });
            return this.lineChartWidgetClient.update(data);
        } else if (widgetType === 'SimpleNumber') {
            const data = UpdateWidgetCommandOfSimpleNumberWidget.fromJS({ id: widgetId, name: widgetName, widgetData });
            return this.simpleNumberWidgetClient.update(data);
        }
    }
}
