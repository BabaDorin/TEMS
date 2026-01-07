import { TokenService } from './token.service';
import { TEMSService } from './tems.service';
import { Injectable, OnInit } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ClaimService extends TEMSService {

  public canManageAssets: boolean = false;
  public canManageTickets: boolean = false;
  public canOpenTickets: boolean = false;

  constructor(
    private tokenService: TokenService
  ) {
    super();

    this.canManageAssets = this.tokenService.canManageAssets();
    this.canManageTickets = this.tokenService.canManageTickets();
    this.canOpenTickets = this.tokenService.canOpenTickets();
  }
}
