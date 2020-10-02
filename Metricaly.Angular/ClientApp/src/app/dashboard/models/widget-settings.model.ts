import { AxisType, AxisSettings } from "./axis-settings.model"

export class WidgetSettings {
  public title: string
  public displayTitle: boolean

  public legendPosition: string;
  public displayLegend: boolean

  public xAxisSettings: AxisSettings = new AxisSettings(AxisType.X)
  public yLeftAxisSettings: AxisSettings = new AxisSettings(AxisType.YLeft)
  public yRightAxisSettings: AxisSettings = new AxisSettings(AxisType.YRight)
}
