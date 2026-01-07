import { HttpInterceptorFn, HttpErrorResponse, HttpRequest, HttpHandlerFn } from '@angular/common/http';
import { inject, Injector } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { Router } from '@angular/router';
import { catchError, throwError, switchMap, from, Observable } from 'rxjs';
import { SnackService } from './services/snack.service';

let isRefreshing = false;

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
      if (error.status === 401 && !isRefreshing) {
        return handle401Error(req, next, oauthService, injector, snackService);
      }
      
      if (error.status === 403) {
        snackService.snack({message: "Insufficient permissions", status: 0});
      }
      
      return throwError(() => error);
    })
  );
};

function handle401Error(
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
  oauthService: OAuthService,
  injector: Injector,
  snackService: SnackService
): Observable<any> {
  isRefreshing = true;
  
  const refreshToken = oauthService.getRefreshToken();
  
  if (!refreshToken) {
    isRefreshing = false;
    redirectToLogin(injector, snackService);
    return throwError(() => new Error('No refresh token available'));
  }
  
  return from(oauthService.refreshToken()).pipe(
    switchMap(() => {
      isRefreshing = false;
      const newToken = oauthService.getAccessToken();
      
      const clonedReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${newToken}`
        }
      });
      
      return next(clonedReq);
    }),
    catchError((refreshError) => {
      isRefreshing = false;
      console.error('[AuthInterceptor] Token refresh failed:', refreshError);
      redirectToLogin(injector, snackService);
      return throwError(() => refreshError);
    })
  );
}

function redirectToLogin(injector: Injector, snackService: SnackService): void {
  const router = injector.get(Router);
  snackService.snack({message: "Session expired, please login again", status: 0});
  localStorage.clear();
  sessionStorage.clear();
  router.navigate(['/login']);
}