import { TokenService } from './token.service';
import { TEMSService } from './tems.service';
import { Injectable, OnInit } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ClaimService extends TEMSService {

  public canManage: boolean = false;
  public canView: boolean = false;
  public canAllocateKeys: boolean = false;
  public canManageSystemConfiguration: boolean = false;
  public canSendEmails: boolean = false;
  public canManageAnnouncements: boolean = false;

  constructor(
    private tokenService: TokenService
  ) {
    super();

    this.canManageAnnouncements = this.tokenService.canManageAnnouncements();
    this.canManage = this.tokenService.canManageEntities(); 
    this.canView = this.tokenService.canViewEntities();
    this.canAllocateKeys = this.tokenService.canAllocateKeys();
    this.canManageSystemConfiguration = this.tokenService.canManageSystemConfiguration();
    this.canSendEmails = this.tokenService.canSendEmails();
  }
}
