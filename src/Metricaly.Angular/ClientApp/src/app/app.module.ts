import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';

import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { environment } from 'src/environments/environment';
import { ChartsModule } from 'ng2-charts';

import {
  NbThemeModule,
  NbChatModule,
  NbDatepickerModule,
  NbDialogModule,
  NbMenuModule,
  NbSidebarModule,
  NbToastrModule,
  NbWindowModule,
  NbLayoutModule,
  NbButtonModule,
  NbTabsetModule,
  NbCardModule,
  NbIconModule,
  NbListModule,
  NbInputModule,
  NbToggleModule,
  NbSelectModule,
  NbFormFieldModule,
  NbCalendarRangeModule,
  NbCalendarModule,
  NbSpinnerModule,
  NbTooltipModule,
  NbContextMenuModule,
  NbAlertModule,
  NbRadioModule,
  NbCheckboxModule
} from '@nebular/theme';

import { NbEvaIconsModule } from '@nebular/eva-icons';
import { NgxEchartsModule } from 'ngx-echarts';


import { AppComponent } from './app.component';
import { HomeComponent } from './views/home/home.component';
import { AppRoutingModule } from './app.routing';
import { JwtInterceptor, ErrorInterceptor } from './core/shared/utils';
import { SharedModule } from './views/shared/shared.module';
import { NgxDaterangepickerMd } from 'ngx-daterangepicker-material';
import { CoreModule } from './core/core.module';
import { API_BASE_URL } from './web-api-client';
import { AppConfigService } from './core/shared/services/appconfig.service';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
  ],
  imports: [
    FormsModule,
    BrowserModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    HttpClientModule,
    AppRoutingModule,
    NbLayoutModule,
    NbButtonModule,
    NbTabsetModule,
    NbCardModule,
    NbThemeModule.forRoot(),
    NbIconModule,
    NbListModule,
    NbDialogModule.forRoot(),
    NbEvaIconsModule,
    NbInputModule,
    NbToggleModule,
    NbSidebarModule.forRoot(),
    NbMenuModule.forRoot(),
    NbSelectModule,
    NbFormFieldModule,
    NbCalendarRangeModule,
    NbCalendarModule,
    NbSpinnerModule,
    NbToastrModule.forRoot({ destroyByClick: true }),
    NbTooltipModule,
    NbContextMenuModule,
    NbAlertModule,
    NbRadioModule,
    NbCheckboxModule,

    NgxDaterangepickerMd.forRoot(),

    NgxEchartsModule.forRoot({
      echarts: () => import('echarts')
    }),

    SharedModule,
    CoreModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    {
      provide: APP_INITIALIZER,
      multi: true,
      deps: [AppConfigService],
      useFactory: (appConfigService: AppConfigService) => {
        return () => {
          // Make sure to return a promise
          return appConfigService.loadAppConfig();
        };
      }
    },
    {
      provide: API_BASE_URL,
      useFactory: () => {
        return environment.apiBaseUrl;
      },
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
