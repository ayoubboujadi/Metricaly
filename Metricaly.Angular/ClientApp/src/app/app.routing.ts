import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
  
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';

import { AuthGuard } from './shared/utils/auth.guard';
import { AppComponent } from './app.component';

const routes: Routes = [
  // { path: '', redirectTo: '/app/home', pathMatch: 'full' },
  {
    path: 'account',
    loadChildren: () => import('./account/account.module').then(m => m.AccountModule), // Lazy load account module
    data: { preload: true }
  },
  {
    path: 'dashboard',
    loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule), // Lazy load account module
    data: { preload: true }
  },
  { path: '', component: AppComponent, pathMatch: 'full' },

  // { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'counter', component: CounterComponent },
  //{ path: 'fetch-data', component: FetchDataComponent },

  //{ path: '', component: HomeComponent, canActivate: [AuthGuard] },

  // otherwise redirect to home
  //{ path: '**', redirectTo: '' }
];

//export const appRoutingModule = RouterModule.forRoot(routes);
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: []
})
export class AppRoutingModule { }
