import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NbMenuItem, NbSidebarService, NbMenuService } from '@nebular/theme';
import { filter, map } from 'rxjs/operators';

import { AuthenticationService } from '@app/core/auth/services';
import { DashboardClient } from '@app/web-api-client';

@Component({
  selector: 'app-views',
  templateUrl: './views.component.html',
  styleUrls: ['./views.component.css']
})
export class ViewsComponent implements OnInit {

  title = 'app';
  state = 'expanded';

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
          link: '/dashboard-page/create',
        },
        {
          title: 'Manage Dashboards',
          link: '/dashboard-page/manage',
        },
      ]
    },

    {
      title: 'Widget',
      icon: 'bar-chart-2-outline',
      children: [
        {
          title: 'Create Widget',
          link: '/widget/create',
        },
        {
          title: 'Manage Widgets',
          link: '/widget/manage',
        }
      ]
    },

    {
      title: 'Application',
      link: '/application',
      icon: 'hard-drive-outline'
    },
    {
      group: true,
      title: 'Starred Dashboards'
    },
  ];

  profileMenuItems = [
    { title: 'Profile' },
    { title: 'Logout' },
  ];

  constructor(private sidebar: NbSidebarService, private dashboardClient: DashboardClient, private nbMenuService: NbMenuService,
    private authenticationService: AuthenticationService) {
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

  ngOnInit() {
    this.nbMenuService.onItemClick()
      .pipe(
        filter(({ tag }) => tag === 'user-context-menu'),
        map(({ item: { title } }) => title),
      )
      .subscribe(title => {
        if (title === 'Logout') {
          this.logoutUser();
        }
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

  logoutUser() {
    this.authenticationService.logout();
  }
}
