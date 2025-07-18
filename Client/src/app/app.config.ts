import { ApplicationConfig, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import {provideToastr} from 'ngx-toastr';
import { errorInterceptor } from './_interceptors/error.interceptor';
import { jwtInterceptor } from './_interceptors/jwt.interceptor';
import {NgxSpinnerModule} from 'ngx-spinner';
import { loadingInterceptor } from './_interceptors/loading.interceptor';
import { TimeagoModule } from 'ngx-timeago';
export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    // provideHttpClient(),
    provideHttpClient(withInterceptors([errorInterceptor
      , jwtInterceptor, loadingInterceptor])),
    //This was added to make drppdown to function
    provideAnimations(),
    //This was added after installing ngx-toastr
    provideToastr({
      positionClass: 'toast-top-center',
      timeOut: 6000,
      preventDuplicates: true,
      closeButton: false
    }),
    importProvidersFrom(NgxSpinnerModule,TimeagoModule.forRoot())
  ],
};
