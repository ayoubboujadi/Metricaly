import { trigger, transition, style, query, animate, animateChild, group } from '@angular/animations';


export const routeTransitionAnimations =
  trigger('routeAnimations', [
    transition('* <=> *', [
      query(':enter',
        [
          style({ opacity: 0 })
        ],
        //{ optional: true }
      ),

      query(':leave',
        [
          style({ opacity: 1 }),
          animate('0.2s', style({ opacity: 0 }))
        ],
        //{ optional: true }
      ),

      query(':enter',
        [
          style({ opacity: 0 }),
          animate('0.2s', style({ opacity: 1 }))
        ],
        //{ optional: true }
      )
    ])
  ]);
