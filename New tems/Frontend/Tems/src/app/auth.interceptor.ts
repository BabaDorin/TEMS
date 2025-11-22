import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject, Injector } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SnackService } from './services/snack.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const oauthService = inject(OAuthService);
  const injector = inject(Injector);
  const snackService = inject(SnackService);
  
  const token = oauthService.getAccessToken();
  
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
  
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Lazy inject Router to avoid circular dependency
        const router = injector.get(Router);
        // Try silent refresh
        oauthService.silentRefresh().then(() => {
          // Token refreshed successfully, but we don't retry here for simplicity
          snackService.snack({message: "Session expired, please login again", status: 0});
          router.navigate(['/login']);
        }).catch(() => {
          snackService.snack({message: "Session expired, please login again", status: 0});
          router.navigate(['/login']);
        });
      }
      
      if (error.status === 403) {
        snackService.snack({message: "Insufficient permissions", status: 0});
      }
      
      // Handle other errors
      snackService.snackIfError(error);
      
      return throwError(() => error);
    })
  );
};