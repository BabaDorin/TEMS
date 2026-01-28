import { CAN_MANAGE_ASSETS, CAN_MANAGE_TICKETS, CAN_OPEN_TICKETS } from '../models/claims';
import { Injectable, inject } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private oauthService = inject(OAuthService);

  constructor() { }

  hasClaim(claimType: string): boolean {
    const claims = this.oauthService.getIdentityClaims() as any;
    if (!claims) return false;
    
    // Map claim constants to role names
    const claimMap: Record<string, string> = {
      [CAN_MANAGE_ASSETS]: 'can_manage_assets',
      [CAN_MANAGE_TICKETS]: 'can_manage_tickets',
      [CAN_OPEN_TICKETS]: 'can_open_tickets'
    };
    
    const roleName = claimMap[claimType] || claimType;
    
    // Keycloak puts roles in multiple places, check all of them
    // 1. Check top-level 'roles' claim (added by our custom mapper)
    if (claims.roles && Array.isArray(claims.roles)) {
      if (claims.roles.includes(roleName)) return true;
    }
    
    // 2. Check realm_access.roles (standard Keycloak location)
    if (claims.realm_access?.roles && Array.isArray(claims.realm_access.roles)) {
      if (claims.realm_access.roles.includes(roleName)) return true;
    }
    
    // 3. Fallback: check as direct claim (for backwards compatibility)
    if (claims[roleName] === 'true' || claims[roleName] === true) {
      return true;
    }
    
    return false;
  }

  getTokenObject(){
    return this.oauthService.getIdentityClaims();
  }

  getUserId(){
    const claims = this.oauthService.getIdentityClaims() as any;
    return claims?.sub || undefined;
  }

  tokenExists(){
    return this.oauthService.hasValidAccessToken();
  }

  getClaimValue(claim: string) {
    const claims = this.oauthService.getIdentityClaims() as any;
    return claims?.[claim];
  }

  // Asset Management permissions
  canManageAssets() {
    return this.hasClaim(CAN_MANAGE_ASSETS);
  }

  // Ticket Management permissions
  canManageTickets() {
    return this.hasClaim(CAN_MANAGE_TICKETS);
  }

  canOpenTickets() {
    return this.hasClaim(CAN_OPEN_TICKETS) || this.hasClaim(CAN_MANAGE_TICKETS);
  }
}
