import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { OAuthService, OAuthEvent } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from '../app.config';
import { BehaviorSubject, Observable, filter, distinctUntilChanged } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$: Observable<boolean> = this.isAuthenticatedSubject.asObservable().pipe(
    distinctUntilChanged()
  );

  constructor(
    private oauthService: OAuthService,
    private router: Router
  ) {
    this.configure();
  }

  private configure() {
    this.oauthService.configure(authCodeFlowConfig);
    
    const initialAuthState = this.oauthService.hasValidAccessToken();
    console.log('[AuthService] Initial auth state:', initialAuthState);
    this.isAuthenticatedSubject.next(initialAuthState);
    
    this.oauthService.events.pipe(
      filter((event: OAuthEvent) => {
        const meaningfulEvents = [
          'token_received',
          'token_refreshed',
          'token_expires',
          'token_error',
          'logout',
          'session_terminated',
          'session_error'
        ];
        return meaningfulEvents.includes(event.type);
      })
    ).subscribe((event: OAuthEvent) => {
      const hasValidToken = this.oauthService.hasValidAccessToken();
      console.log('[AuthService] Meaningful event:', event.type, 'hasValidToken:', hasValidToken);
      this.isAuthenticatedSubject.next(hasValidToken);
    });
    
    this.oauthService.loadDiscoveryDocumentAndTryLogin().then(() => {
      const hasValidToken = this.oauthService.hasValidAccessToken();
      console.log('[AuthService] After loadDiscoveryDocumentAndTryLogin, hasValidToken:', hasValidToken);
      this.isAuthenticatedSubject.next(hasValidToken);
    });
  }

  logIn() {
    this.oauthService.initCodeFlow('', { kc_idp_hint: 'duende-idp' });
  }

  signOut(): void {
    // TODO: Consider re-enabling this for better logout handling
    // to also expire the token on the server side.
    // this.oauthService.logOut(false);
    localStorage.removeItem('token');
    sessionStorage.clear();
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/home']);
  }

  isLoggedIn(): boolean {
    return this.oauthService.hasValidAccessToken();
  }

  getAccessToken(): string {
    return this.oauthService.getAccessToken();
  }

  getIdentityClaims(): any {
    return this.oauthService.getIdentityClaims();
  }

  getUserName(): string {
    const claims = this.getIdentityClaims();
    if (claims) {
      return claims.name || claims.preferred_username || claims.email || 'User';
    }
    return '';
  }
}
