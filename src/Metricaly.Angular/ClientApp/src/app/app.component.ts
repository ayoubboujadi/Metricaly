import { Component } from '@angular/core';
import { NbMenuItem, NbSidebarService } from '@nebular/theme';
import { RouterOutlet } from '@angular/router';
import { routeTransitionAnimations } from './route-transition-animations';
import { DashboardClient } from '@app/web-api-client';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  animations: [routeTransitionAnimations]
})
export class AppComponent {
  title = 'app';
  menuItems: NbMenuItem[] = [
    {
      title: 'Home',
      link: '/',
      icon: 'home-outline',
    },

    {
      title: 'Dashboard',
      icon: 'monitor-outline',
      children: [
        {
          title: 'Create Dashboard',
          link: 'dashboard/create',
        },
        {
          title: 'Manage Dashboards',
          link: 'dashboard/manage',
        },
      ]
    },

    {
      title: 'Widget',
      icon: 'bar-chart-2-outline',
      children: [
        {
          title: 'Create Widget',
          link: 'widget-builder',
        },
        {
          title: 'Manage Widgets',
          link: 'widget-builder/manage',
        }
      ]
    },

    {
      title: 'Application',
      link: 'application',
      icon: 'hard-drive-outline'
    },
    {
      group: true,
      title: 'Starred Dashboards'
    },
  ];
  state = 'expanded';

  constructor(private sidebar: NbSidebarService, private dashboardClient: DashboardClient) {
    const menuState = localStorage.getItem('menuState');
    if (!menuState) {
      localStorage.setItem('menuState', this.state);
    } else {
      this.state = menuState;
    }

    this.dashboardClient.getFavoriteList()
      .subscribe((result) => {
        result.forEach(dashboard => this.menuItems.push({
          title: dashboard.name,
          link: 'dashboard/view/' + dashboard.id,
          icon: 'star-outline',
        }));
      });
  }

  // For root animations
  prepareRoute(outlet: RouterOutlet) {
    return outlet.isActivated ? outlet.activatedRoute : '';
  }

  toggleMenu() {
    if (this.state === 'expanded') {
      this.sidebar.compact();
      this.state = 'compacted';
      localStorage.setItem('menuState', 'compacted');
    } else {
      this.sidebar.expand();
      this.state = 'expanded';
      localStorage.setItem('menuState', 'expanded');
    }
  }
}
