import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import {provideToastr} from 'ngx-toastr';
import { errorInterceptor } from './_interceptors/error.interceptor';
export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    // provideHttpClient(),
    provideHttpClient(withInterceptors([errorInterceptor])),
    //This was added to make drppdown to function
    provideAnimations(),
    //This was added after installing ngx-toastr
    provideToastr({
      positionClass: 'toast-top-center'
    })
  ],
};
