import { CAN_MANAGE_ANNOUNCEMENTS } from './../models/claims';
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { TokenService } from '../services/token-service/token.service';

@Injectable({
  providedIn: 'root'
})
export class CanManageAnnouncementsGuard implements CanActivate {
  constructor(
    private tokenService: TokenService
  ){

  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    return this.tokenService.hasClaim(CAN_MANAGE_ANNOUNCEMENTS); 
  }
  
}
