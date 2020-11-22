import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DummyDataGeneratorService {

  private samplingValue: number = 300; // in seconds, 300 seconds = 5 min
  private dummyData: number[][] = []


  constructor() {
    var dummyDataCount = 4
    var dummyDataSize = 3000

    for (var count = 0; count <= dummyDataCount; count += 1) {

      var startValue = this.randomIntFromInterval(500, 3000)

      this.dummyData.push([])

      for (var i = 0; i <= dummyDataSize; i += 1) {

        this.dummyData[count].push(startValue);

        startValue = this.randomIntFromInterval(startValue - 100, startValue + 100)
        if (startValue < 0)
          startValue = 0
      }
    }

    //console.log(this.dummyData)
  }

  getData(startTime: number, metricGuids: string[]): TimeSeriesResponse {
    var currentSeconds = Date.now() / 1000
    var granularity = 5 // 5 min

    var currentTimestamp = Math.floor(currentSeconds / granularity) * granularity
    startTime = Math.floor(startTime / granularity) * granularity

    var response = new TimeSeriesResponse

    metricGuids.forEach(metricGuid => response.metricsTimeSeriesData.push(new MetricTimeSeriesData(metricGuid)))

    // Add timestamps
    for (var i = startTime; i <= currentTimestamp; i += granularity) {
      response.timestamps.push(i)
    }

    // Add Data
    response.metricsTimeSeriesData.forEach((metric, index) => {
      for (var i = 0; i < response.timestamps.length; i++) {
        metric.data.push(this.dummyData[index][i])
      }
    })

    return response
    // Every 10 seconds call the API and get the data for the current widget
    // Data should be between the start time and end time, if no end time is provided it should be NOW
    // 
  }

  randomIntFromInterval(min: number, max: number) { // min and max included 
    return Math.floor(Math.random() * (max - min + 1) + min);
  }

}


export class TimeSeriesResponse {
  public timestamps: number[] = []
  public metricsTimeSeriesData: MetricTimeSeriesData[] = []
}

export class MetricTimeSeriesData {

  constructor(metricGuid: string) {
    this.metricGuid = metricGuid
  }

  public metricGuid: string
  public data: number[] = []
}
