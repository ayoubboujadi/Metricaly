import { ViewsComponent } from './views.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '@app/core/shared/utils';
import { HomeComponent } from './home/home.component';

const routes: Routes = [{
  path: '',
  component: ViewsComponent,
  children: [
    {
      path: 'dashboard',
      loadChildren: () => import('./dashboard/dashboard.module')
        .then(m => m.DashboardModule), // Lazy load account module
      data: { preload: true, breadcrumb: 'Dashboard' },
      canActivate: [AuthGuard]
    },
    {
      path: 'widget-builder',
      loadChildren: () => import('./widget-builder/widget-builder.module')
        .then(m => m.WidgetBuilderModule), // Lazy load widget-builder
      data: { preload: true },
      canActivate: [AuthGuard]
    },
    {
      path: 'application',
      loadChildren: () => import('./pages/application/application.module')
        .then(m => m.ApplicationModule), // Lazy load application module
      data: { preload: true },
      canActivate: [AuthGuard]
    },
    {
      path: 'widget',
      loadChildren: () => import('./pages/widget/widget-page.module')
        .then(m => m.WidgetPageModule), // Lazy load widget module
      data: { preload: true, breadcrumb: 'Widget' },
      canActivate: [AuthGuard]
    },
    {
      path: 'dashboard-page',
      loadChildren: () => import('./pages/dashboard/dashboard-page.module')
        .then(m => m.DashboardPageModule), // Lazy load widget module
      data: { preload: true, breadcrumb: 'Dashboard' },
      canActivate: [AuthGuard]
    },
    {
      path: '',
      component: HomeComponent,
      pathMatch: 'full',
      canActivate: [AuthGuard]
    },
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: []
})
export class ViewsRoutingModule { }
