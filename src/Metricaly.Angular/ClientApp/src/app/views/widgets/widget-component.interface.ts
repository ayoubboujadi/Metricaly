import { TimePeriod } from './../../core/shared/models/timeperiod.model';


export abstract class WidgetComponent {
    widgetData: any;
    timePeriod: TimePeriod;
    applicationId: string;

    abstract hardReloadPlottedMetrics(): void;
    abstract loadPlottedMetricsData(): void;
}