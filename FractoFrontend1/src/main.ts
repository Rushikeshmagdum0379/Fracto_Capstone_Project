import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app/app.routes';
import {App } from './app/app';
bootstrapApplication(App, {
  providers: [
    provideRouter(routes),
    provideHttpClient(),   //  This fixes the HttpClient injection
  ],
});
