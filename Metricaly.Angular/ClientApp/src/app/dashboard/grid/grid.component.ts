import { Component, OnInit, ContentChildren, QueryList, AfterViewInit, ElementRef, Renderer2 } from '@angular/core';
import { GridItemComponent } from '../grid-item/grid-item.component';
import { GridStack } from 'gridstack';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'div[gridstack]',
  templateUrl: './grid.component.html',
  styleUrls: ['./grid.component.css']
})
export class GridComponent implements OnInit, AfterViewInit {

  @ContentChildren(GridItemComponent) public gridstackItems: QueryList<GridItemComponent>;

  private gridstack: GridStack;
  private _ngUnsubscribe = new Subject();
  private gridItems: GridItem[] = [];

  constructor(private element: ElementRef, private renderer: Renderer2) { }

  ngOnInit() {
    this.renderer.addClass(this.element.nativeElement, 'grid-stack');
    this.renderer.addClass(this.element.nativeElement, 'grid-stack-6');

    this.gridstack = GridStack.init({ column: 6 }, this.element.nativeElement);

    this.gridstack.on('added', (evt: any, items: Item[]) => {
      console.log('added fired')
    });

    this.gridstack.on('change', (evt: any, items: Item[]) => {
      console.log('change fired')
      items.forEach(changedItem => {
        var itemToUpdate = this.gridstackItems.find(itemComponent => itemComponent.id == changedItem.id)
        itemToUpdate.setItemNewValues(changedItem.x, changedItem.y, changedItem.width, changedItem.height)
      });
    });

    this.gridstack.on('removed', (evt: any, items: Item[]) => {
      console.log('removed fired')
    });
  }

  public ngAfterViewInit(): void {
    console.log('ngAfterViewInit() called');

    this.gridstackItems.changes
      .pipe(takeUntil(this._ngUnsubscribe))
      .subscribe(changes => {
        this.handleItemChanges(this.gridstackItems.toArray());
      });

    this.handleItemChanges(this.gridstackItems.toArray());
  }

  private handleItemChanges(items: GridItemComponent[]): void {
    console.log('_handleItemChanges: ' + items.length);

    const itemsOfCurrentGrid = this.gridItems;
    const itemsToAdd = items.filter(i => !itemsOfCurrentGrid.some(w => w.generatedId === i.id));
    const itemsToRemove = itemsOfCurrentGrid.filter(w => !items.some(i => i.id === w.generatedId));

    this.gridstack.batchUpdate();
    itemsToAdd.forEach(i => this.addItem(i));
    itemsToRemove.forEach(i => this.removeItem(i));
    this.gridstack.commit();
  }

  private addItem(item: GridItemComponent): void {
    if (this.gridstack.willItFit(+item.x, +item.y, +item.width, +item.height, true)) {
      this.gridstack.makeWidget(item.elem.nativeElement);
      this.gridItems.push({
        generatedId: item.id,
        elem: item.elem
      });
      console.log('Added item with id:' + item.id);
    } else {
      console.error('Not enough free space to place the widget');
    }
  }

  private removeItem(item: GridItem): void {
    this.gridItems.splice(this.gridItems.findIndex(gi => gi.generatedId == item.generatedId), 1);
    this.gridstack.removeWidget(item.elem.nativeElement);
  }


}


export interface GridItem {
  generatedId: string;
  elem: ElementRef;
}


export interface Item {
  autoPosition: boolean;
  height: number;
  id: string | null;
  locked: boolean;
  maxHeight: number | null;
  maxWidth: number | null;
  minHeight: number | null;
  minWidth: number | null;
  noMove: boolean;
  noResize: boolean;
  width: number;
  x: number;
  y: number;
  el: any;
}
