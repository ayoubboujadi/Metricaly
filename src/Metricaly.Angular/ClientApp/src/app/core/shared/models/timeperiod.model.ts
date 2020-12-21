
export class TimePeriod {

  public constructor(start: number, end: number) {
    this.start = start
    this.end = end
  }

  public start: number | any
  public end: number | any
  public liveSpan: string | any | null = null
}
