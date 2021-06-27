import { CAN_VIEW_ENTITIES, CAN_MANAGE_ENTITIES, CAN_MANAGE_SYSTEM_CONFIGURATION, CAN_ALLOCATE_KEYS, CAN_SEND_EMAILS } from '../models/claims';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  constructor() { }

  hasClaim(claim: string): boolean {
    let token = localStorage.getItem('token');
    if (token == null) return false;

    return JSON.parse(window.atob(token.split('.')[1]))[claim] != undefined;
  }

  getClaimValue(claim: string) {
    let token = localStorage.getItem('token');
    if (token == null) return false;

    return JSON.parse(window.atob(token.split('.')[1]))[claim];
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
}
