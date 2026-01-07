import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter, withPreloading, NoPreloading } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { TranslateModule, TranslateLoader, TranslatePipe } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient } from '@angular/common/http';
import { AuthConfig, provideOAuthClient } from 'angular-oauth2-oidc';
import { environment } from '../environments/environment';
import { authInterceptor } from './auth.interceptor';
import { CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
import { PaginatePipe } from 'ngx-pagination';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ThemeService } from 'ng2-charts';

import { RoleService } from './services/role.service';
import { DialogService } from './services/dialog.service';
import { RoomsService } from './services/rooms.service';
import { PersonnelService } from './services/personnel.service';
import { AssetService } from './services/asset.service';
import { TokenService } from './services/token.service';
import { ClaimService } from './services/claim.service';
import { MenuService } from './services/menu.service';
import { TypeService } from './services/type.service';
import { DefinitionService } from './services/definition.service';
import { LazyLoaderService } from './services/lazy-loader.service';
import { DownloadService } from './download.service';

import { routes } from './app-routing.module';

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

export const authCodeFlowConfig: AuthConfig = {
  issuer: `${environment.keycloakUrl}/realms/${environment.keycloakRealm}`,
  redirectUri: window.location.origin + '/callback',
  postLogoutRedirectUri: window.location.origin + '/home',
  clientId: 'tems-angular-spa',
  responseType: 'code',
  scope: 'openid profile email roles offline_access',
  showDebugInformation: !environment.production,
  
  // Token Refresh Configuration
  useSilentRefresh: false, // We use refresh tokens instead of silent refresh iframe
  silentRefreshRedirectUri: window.location.origin + '/silent-refresh.html',
  
  // Session Configuration
  sessionChecksEnabled: false,
  
  // Token Lifecycle - Refresh 60 seconds before expiry
  timeoutFactor: 0.75, // Refresh when 75% of token lifetime has passed
  
  // Security
  requireHttps: false, // Allow HTTP for local development
  
  // Disable PKCE for Keycloak compatibility (Keycloak handles security via client config)
  disablePKCE: false,
};

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withPreloading(NoPreloading)),
    provideAnimations(),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideOAuthClient(),
    importProvidersFrom(
      TranslateModule.forRoot({
        defaultLanguage: 'en',
        loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
        }
      })
    ),
    // Services from app.module.ts
    RoleService,
    ThemeService,
    DialogService,
    RoomsService,
    PersonnelService,
    AssetService,
    TokenService,
    ClaimService,
    MenuService,
    MatSnackBar,
    TypeService,
    DefinitionService,
    CurrencyPipe,
    DatePipe,
    PaginatePipe,
    DecimalPipe,
    TranslatePipe,
    LazyLoaderService,
    DownloadService,
  ]
};
