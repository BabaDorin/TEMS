import { TokenService } from './token-service/token.service';
import { TEMSService } from './tems-service/tems.service';
import { Injectable, OnInit } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ClaimService extends TEMSService {

  public canManage: boolean = false;
  public canView: boolean = false;
  public canAllocateKeys: boolean = false;
  public canManageSystemConfiguration: boolean = false;

  constructor(
    private tokenService: TokenService
  ) {
    super();

    console.log('can manage: ');
    console.log(this.tokenService.canManageEntities());
    
    this.canManage = this.tokenService.canManageEntities(); 
    this.canView = this.tokenService.canViewEntities();
    this.canAllocateKeys = this.tokenService.canAllocateKeys();
    this.canManageSystemConfiguration = this.tokenService.canManageSystemConfiguration();
  }
}
