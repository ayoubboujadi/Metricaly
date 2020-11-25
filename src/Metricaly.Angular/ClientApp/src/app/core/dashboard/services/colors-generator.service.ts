import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ColorsGeneratorService {

  constructor() { }

  // from https://sashamaps.net/docs/resources/20-colors/
  private colors: string[] = [
    '#e6194B',
    '#3cb44b',
    '#ffe119',
    '#4363d8',
    '#f58231',
    '#911eb4',
    '#42d4f4',
    '#f032e6',
    '#bfef45',
    '#fabed4',
    '#469990',
    '#dcbeff',
    '#9A6324',
    '#fffac8',
    '#800000',
    '#aaffc3',
    '#808000',
    '#ffd8b1',
    '#000075',
    '#a9a9a9'
  ];

  public getColor(except: string[] = []): string {
    const availableColors = this.colors.filter(color => except.indexOf(color) < 0);
    if (availableColors.length > 0) {
      return availableColors[0];
    }

    return '#' + (0x1000000 + (Math.random()) * 0xffffff).toString(16).substr(1, 6);
  }
}
