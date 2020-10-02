export enum AxisType {
  X,
  YLeft,
  YRight
}

export class AxisSettings {

  constructor(type: AxisType) {
    this.type = type
  }

  public label: string;
  public displayLabel: boolean
  public displayGridLines: boolean
  public type: AxisType
}
