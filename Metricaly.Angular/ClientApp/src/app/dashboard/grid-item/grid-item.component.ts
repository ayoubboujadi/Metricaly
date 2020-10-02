import { Component, OnInit, Input, AfterViewInit, ElementRef, Renderer2, Output, EventEmitter } from '@angular/core';
import { Item } from '../grid/grid.component';


@Component({
  selector: 'div[grid-item]',
  templateUrl: './grid-item.component.html',
  styleUrls: ['./grid-item.component.css']
})
export class GridItemComponent implements OnInit, AfterViewInit {

  @Input() public item: Item
  @Output() itemChange: EventEmitter<Item> = new EventEmitter<Item>();

  constructor(public elem: ElementRef, private renderer: Renderer2) {
  }


  ngAfterViewInit(): void {
    this.renderer.addClass(this.elem.nativeElement, 'grid-stack-item');

    this.setAttributeIfNotUndefined('data-gs-x', this.item.x);
    this.setAttributeIfNotUndefined('data-gs-y', this.item.y);
    this.setAttributeIfNotUndefined('data-gs-width', this.item.width);
    this.setAttributeIfNotUndefined('data-gs-height', this.item.height);
    this.setAttributeIfNotUndefined('data-gs-id', this.item.id);

    console.log("GridItemComponent ngAfterViewInit for ID: " + this.item.id)
  }

  private setAttributeIfNotUndefined(attrName: string, val: number | string): void {
    if (val == null)
      return;

    this.renderer.setAttribute(this.elem.nativeElement, attrName, val.toString());
  }

  ngOnInit() {
  }

  setItemNewValues(x: number, y: number, width: number, height: number) {
    this.item.x = x;
    this.item.y = y;
    this.item.width = width;
    this.item.height = height;

    this.itemChange.emit(this.item);
  }

  set x(value: number) {
    this.item.x = value;
    this.itemChange.emit(this.item);
  }

  set y(value: number) {
    this.item.y = value;
    this.itemChange.emit(this.item);
  }

  set width(value: number) {
    this.item.width = value;
    this.itemChange.emit(this.item);
  }

  set height(value: number) {
    this.item.height = value;
    this.itemChange.emit(this.item);
  }

  get id(): string {
    return this.item.id
  }
}
