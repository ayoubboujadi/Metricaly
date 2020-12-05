import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AppConfigService {

  private appConfig: any;

  constructor(private http: HttpClient) { }

  loadAppConfig() {
    let configFile = '/assets/config.dev.json';
    if (environment.production) {
      configFile = '/assets/config.prod.json';
    }

    return this.http.get(configFile)
      .toPromise()
      .then(data => {
        this.appConfig = data;
      });
  }

  get apiBaseUrl() {
    if (!this.appConfig) {
      throw Error('Config file not loaded!');
    }

    return this.appConfig.apiBaseUrl;
  }
}