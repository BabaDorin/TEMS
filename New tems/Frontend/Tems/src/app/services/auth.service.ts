import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { authCodeFlowConfig } from '../app.config';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(
    private oauthService: OAuthService,
    private router: Router
  ) {
    this.configure();
  }

  private configure() {
    this.oauthService.configure(authCodeFlowConfig);
    this.oauthService.loadDiscoveryDocumentAndTryLogin().then(() => {
      if (this.oauthService.hasValidAccessToken()) {
        // User is already logged in, can navigate to app
        const currentPath = this.router.url;
        if (currentPath === '/login' || currentPath === '/') {
          this.router.navigate(['/tems']);
        }
      }
    });
  }

  logIn() {
    // Add kc_idp_hint to automatically redirect to Duende IdentityServer
    this.oauthService.initCodeFlow('', { kc_idp_hint: 'duende-idp' });
  }

  signOut(): Promise<void> {
    return this.oauthService.revokeTokenAndLogout().then(() => {
      localStorage.removeItem('token'); // Clean up old token if exists
      this.router.navigate(['/login']);
    });
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
}
