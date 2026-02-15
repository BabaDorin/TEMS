import { Injectable, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { OAuthService, OAuthEvent } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from '../app.config';
import { BehaviorSubject, Observable, Subscription, filter, distinctUntilChanged, interval } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService implements OnDestroy {
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$: Observable<boolean> = this.isAuthenticatedSubject.asObservable().pipe(
    distinctUntilChanged()
  );

  private tokenRefreshSubscription?: Subscription;
  private readonly TOKEN_REFRESH_CHECK_INTERVAL = 30000; // Check every 30 seconds
  private readonly TOKEN_REFRESH_THRESHOLD = 60; // Refresh 60 seconds before expiry

  constructor(
    private oauthService: OAuthService,
    private router: Router
  ) {
    this.configure();
  }

  ngOnDestroy(): void {
    this.stopTokenRefreshTimer();
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
          'session_error',
          'silently_refreshed',
          'token_refresh_error'
        ];
        return meaningfulEvents.includes(event.type);
      })
    ).subscribe((event: OAuthEvent) => {
      console.log('[AuthService] OAuth event:', event.type);
      
      if (event.type === 'token_refreshed' || event.type === 'silently_refreshed') {
        console.log('[AuthService] Token successfully refreshed');
        this.isAuthenticatedSubject.next(true);
      } else if (event.type === 'token_refresh_error') {
        console.error('[AuthService] Token refresh failed, user needs to re-authenticate');
        this.handleTokenRefreshError();
      } else {
        const hasValidToken = this.oauthService.hasValidAccessToken();
        console.log('[AuthService] Event:', event.type, 'hasValidToken:', hasValidToken);
        this.isAuthenticatedSubject.next(hasValidToken);
      }
    });
    
    this.oauthService.loadDiscoveryDocumentAndTryLogin().then(() => {
      const hasValidToken = this.oauthService.hasValidAccessToken();
      console.log('[AuthService] After loadDiscoveryDocumentAndTryLogin, hasValidToken:', hasValidToken);
      this.isAuthenticatedSubject.next(hasValidToken);
      
      if (hasValidToken) {
        this.startTokenRefreshTimer();
      }
    });
  }

  private startTokenRefreshTimer(): void {
    this.stopTokenRefreshTimer();
    
    console.log('[AuthService] Starting token refresh timer');
    
    this.tokenRefreshSubscription = interval(this.TOKEN_REFRESH_CHECK_INTERVAL).subscribe(() => {
      this.checkAndRefreshToken();
    });
    
    this.checkAndRefreshToken();
  }

  private stopTokenRefreshTimer(): void {
    if (this.tokenRefreshSubscription) {
      console.log('[AuthService] Stopping token refresh timer');
      this.tokenRefreshSubscription.unsubscribe();
      this.tokenRefreshSubscription = undefined;
    }
  }

  private checkAndRefreshToken(): void {
    if (!this.oauthService.hasValidAccessToken()) {
      console.log('[AuthService] No valid access token, attempting refresh');
      this.refreshToken();
      return;
    }

    const expiresAt = this.oauthService.getAccessTokenExpiration();
    if (!expiresAt) {
      console.log('[AuthService] No expiration time found for access token');
      return;
    }

    const now = Date.now();
    const expiresIn = (expiresAt - now) / 1000;
    
    console.log(`[AuthService] Token expires in ${Math.round(expiresIn)} seconds`);
    
    if (expiresIn < this.TOKEN_REFRESH_THRESHOLD) {
      console.log('[AuthService] Token expiring soon, refreshing...');
      this.refreshToken();
    }
  }

  private async refreshToken(): Promise<void> {
    try {
      const refreshToken = this.oauthService.getRefreshToken();
      
      if (!refreshToken) {
        console.log('[AuthService] No refresh token available');
        this.handleTokenRefreshError();
        return;
      }

      console.log('[AuthService] Refreshing token using refresh token...');
      await this.oauthService.refreshToken();
      console.log('[AuthService] Token refresh successful');
      this.isAuthenticatedSubject.next(true);
      
    } catch (error) {
      console.error('[AuthService] Token refresh failed:', error);
      this.handleTokenRefreshError();
    }
  }

  private handleTokenRefreshError(): void {
    console.log('[AuthService] Handling token refresh error - clearing session');
    this.stopTokenRefreshTimer();
    this.isAuthenticatedSubject.next(false);
  }

  logIn() {
    this.oauthService.initCodeFlow('', { kc_idp_hint: 'duende-idp' });
  }

  signOut(): void {
    console.log('[AuthService] Logging out...');
    
    this.stopTokenRefreshTimer();
    this.oauthService.logOut();
    localStorage.clear();
    sessionStorage.clear();
    this.isAuthenticatedSubject.next(false);
    window.location.href = '/home';
  }

  isLoggedIn(): boolean {
    return this.oauthService.hasValidAccessToken();
  }

  getAccessToken(): string {
    return this.oauthService.getAccessToken();
  }

  getRefreshToken(): string {
    return this.oauthService.getRefreshToken();
  }

  getTokenExpiration(): number | null {
    return this.oauthService.getAccessTokenExpiration();
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

  /**
   * Called by the callback component after OAuth login completes.
   * This ensures the navbar and other components are notified of the auth state change.
   */
  notifyLoginComplete(): void {
    const hasValidToken = this.oauthService.hasValidAccessToken();
    console.log('[AuthService] Login complete notification, hasValidToken:', hasValidToken);
    
    if (hasValidToken) {
      this.isAuthenticatedSubject.next(true);
      this.startTokenRefreshTimer();
    }
  }

  async forceTokenRefresh(): Promise<boolean> {
    try {
      await this.oauthService.refreshToken();
      return true;
    } catch (error) {
      console.error('[AuthService] Force token refresh failed:', error);
      return false;
    }
  }
}
