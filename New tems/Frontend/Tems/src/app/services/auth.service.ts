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
    this.oauthService.initCodeFlow();
  }

  signOut() {
    this.oauthService.revokeTokenAndLogout();
    localStorage.removeItem('token'); // Clean up old token if exists
    this.router.navigate(['/login']);
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
