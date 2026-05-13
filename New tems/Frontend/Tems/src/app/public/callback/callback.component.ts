import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-callback',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="d-flex justify-content-center align-items-center" style="height: 100vh;">
      <div class="text-center">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
        <p class="mt-3">Processing login...</p>
      </div>
    </div>
  `
})
export class CallbackComponent implements OnInit {
  constructor(
    private oauthService: OAuthService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    console.log('[Callback] Processing OAuth callback...');
    console.log('[Callback] Current URL:', window.location.href);

    // AuthService already processes the authorization code during app startup.
    // The callback page should only verify the token and continue, not exchange
    // the same code a second time.
    setTimeout(() => {
      const hasToken = this.oauthService.hasValidAccessToken();
      const accessToken = this.oauthService.getAccessToken();
      console.log('[Callback] Has valid token:', hasToken);
      console.log('[Callback] Access token exists:', !!accessToken);

      if (hasToken && accessToken) {
        console.log('[Callback] Login successful, notifying auth service and redirecting to home');
        this.authService.notifyLoginComplete();
        this.router.navigate(['/home']);
      } else {
        console.error('[Callback] No valid access token after login');
        console.error('[Callback] Token details:', {
          hasValidToken: hasToken,
          tokenExists: !!accessToken,
          claims: this.oauthService.getIdentityClaims()
        });
        this.router.navigate(['/login']);
      }
    }, 100);
  }
}
