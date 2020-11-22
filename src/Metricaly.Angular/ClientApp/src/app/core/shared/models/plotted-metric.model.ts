import { MetricData } from "./metric-data.model"
import { GuidGenerator } from "../utils/guid-generator"

export class PlottedMetric {
  constructor(metricData: MetricData) {
    this.metricId = metricData.id;
    this.metricName = metricData.name;
    this.namespace = metricData.namespace;
    this.label = metricData.namespace + ' ' + metricData.name;
    this.yAxis = 'left';
  }

  public metricId: string;
  public guid: string = GuidGenerator.newGuid();
  public label: string;
  public metricName: string;
  public namespace: string;
  public color: string;
  public yAxis: string; // left or right
  public samplingType = 'Average';
}



