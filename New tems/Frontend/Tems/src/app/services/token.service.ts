import { CAN_MANAGE_ANNOUNCEMENTS } from './../models/claims';
import { CAN_VIEW_ENTITIES, CAN_MANAGE_ENTITIES, CAN_MANAGE_SYSTEM_CONFIGURATION, CAN_ALLOCATE_KEYS, CAN_SEND_EMAILS } from '../models/claims';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  constructor() { }

  hasClaim(claim: string): boolean {
    let token = this.getTokenObject();
    if(token == undefined)
      return false;
      
    return this.getTokenObject()[claim] != undefined;
  }

  getTokenObject(){
    let token = localStorage.getItem('token');
    if(token == undefined)
      return undefined;

    return JSON.parse(window.atob(token.split('.')[1]))
  }

  getUserId(){
    let token = this.getTokenObject();

    if(token == undefined)
      return undefined;
    
    return token.UserID;
  }

  tokenExists(){
    let token = localStorage.getItem('token');
    return token != undefined && token.trim() != 'bearer'; 
  }

  getClaimValue(claim: string) {
    return this.getTokenObject()[claim];
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
