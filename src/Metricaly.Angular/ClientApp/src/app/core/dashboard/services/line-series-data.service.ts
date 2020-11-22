import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthenticationService } from '../../auth/services';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LineSeriesData {

  constructor(private http: HttpClient, private authenticationService: AuthenticationService) { }

  private url = 'https://localhost:44344'

  getMetricsData<MetricTimeSeriesResponse>(data: MetricsDataRequest): Observable<MetricTimeSeriesResponse> {

    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    };

    return this.http.post<MetricTimeSeriesResponse>(this.url + '/metric/get', data, httpOptions)
  }

  getApplicationMetrics(applicationId: number) {
    return this.http.get(this.url + '/metric/' + applicationId + '/metrics')
  }

}

export class MetricsDataRequest {
  startTimestamp: number;
  endTimestamp: number;
  samplingTime: number;
  applicationId: string;
  metrics: Metric[] = [];
}

export class Metric {
  metricName: string;
  namespace: string;
  samplingType: number;
  Guid: string;
}

export class MetricTimeSeriesResponse {
  samplingValue: number;
  count: number;
  timestamps: number[];
  values: Value[];
}

export class Value {
  guid: string;
  metricName: string;
  namespace: string;
  values: Array<number | null>;
}
