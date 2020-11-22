import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { share } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class SubHeaderService {

  private setTitle$ = new Subject<{ title: string }>();


  onSetTitle(): Observable<{ title: string }> {
    return this.setTitle$.pipe(share());
  }

  setTitle(title?: string) {
    this.setTitle$.next({ title });
  }
}
