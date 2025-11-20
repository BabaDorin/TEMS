import { CAN_MANAGE_ANNOUNCEMENTS } from './../models/claims';
import { CAN_VIEW_ENTITIES, CAN_MANAGE_ENTITIES, CAN_MANAGE_SYSTEM_CONFIGURATION, CAN_ALLOCATE_KEYS, CAN_SEND_EMAILS } from '../models/claims';
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
    
    // Map old claim format to new OIDC claim format
    const claimMap: Record<string, string> = {
      [CAN_VIEW_ENTITIES]: 'can_view_entities',
      [CAN_MANAGE_ENTITIES]: 'can_manage_entities',
      [CAN_ALLOCATE_KEYS]: 'can_allocate_keys',
      [CAN_SEND_EMAILS]: 'can_send_emails',
      [CAN_MANAGE_ANNOUNCEMENTS]: 'can_manage_announcements',
      [CAN_MANAGE_SYSTEM_CONFIGURATION]: 'can_manage_system_configuration'
    };
    
    const mappedClaim = claimMap[claimType] || claimType;
    return claims[mappedClaim] === 'true';
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

  canViewEntities() {
    return this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES) || this.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION)
  }

  canManageEntities() {
    return this.hasClaim(CAN_MANAGE_ENTITIES) || this.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION);
  }

  canManageSystemConfiguration() {
    return this.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION);
  }

  canAllocateKeys() {
    return this.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION) || this.hasClaim(CAN_ALLOCATE_KEYS);
  }

  canSendEmails() {
    return this.hasClaim(CAN_SEND_EMAILS) || this.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION);
  }

  canManageAnnouncements(){
    return this.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION) || this.hasClaim(CAN_MANAGE_ANNOUNCEMENTS);
  }
}
