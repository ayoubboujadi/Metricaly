import { Injectable } from '@angular/core';
import { ElementRef } from '@angular/core';

export interface GridItem {
  generatedId: string;
  elem: ElementRef;
}
export interface Grid {
  generatedId: string;
}


@Injectable()
export class GridstackService {
  private _gridItems: GridItem[] = [];

  private _grids: Grid;

  constructor() { }

  public getGridItems() {
    return this._gridItems;
  }

  public removeGridItem(gridItemId: string) {
    this._gridItems.splice(this._gridItems.findIndex(gi => gi.generatedId === gridItemId), 1);
  }
}
