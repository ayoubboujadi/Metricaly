import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    loadChildren: () => import('./views/views.module')
      .then(m => m.ViewsModule),
  },
  {
    path: 'account',
    loadChildren: () => import('./views/account/account.module')
      .then(m => m.AccountModule), // Lazy load account module
    data: { preload: true }
  },
  { path: '', redirectTo: '', pathMatch: 'full' },
  { path: '**', redirectTo: '' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: []
})
export class AppRoutingModule { }
