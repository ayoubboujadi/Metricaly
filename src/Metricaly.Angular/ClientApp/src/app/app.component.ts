import { Component } from '@angular/core';
import { routeTransitionAnimations } from './route-transition-animations';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  animations: [routeTransitionAnimations]
})
export class AppComponent {

  constructor() {
  }
}
