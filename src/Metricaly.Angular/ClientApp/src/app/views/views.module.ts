import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';

import { ViewsComponent } from './views.component';
import { SharedModule } from './shared/shared.module';

// Nebular theme
import {
  NbLayoutModule,
  NbButtonModule,
  NbIconModule,
  NbSidebarModule,
  NbMenuModule,
} from '@nebular/theme';
import { NbEvaIconsModule } from '@nebular/eva-icons';
import { RouterModule } from '@angular/router';
import { ViewsRoutingModule } from './views.routing';


@NgModule({
  declarations: [ViewsComponent],
  imports: [
    ViewsRoutingModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,

    SharedModule,

    NbLayoutModule,
    NbButtonModule,
    NbIconModule,
    NbSidebarModule,
    NbMenuModule,
    NbEvaIconsModule,
  ]
})
export class ViewsModule {

}
