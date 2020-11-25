import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboardFavoriteService {
  private subject = new Subject<boolean>();

  constructor() { }

  getSubscription(): Observable<any> {
    return this.subject.asObservable();
  }

  favoritesChanged() {
    this.subject.next(true);
  }

}
