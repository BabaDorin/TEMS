import { CAN_ALLOCATE_KEYS } from './../models/claims';
import { TokenService } from './../services/token-service/token.service';
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CanAllocateKeysGuard implements CanActivate {
  constructor(
    private tokenService: TokenService
  ){

  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    return this.tokenService.hasClaim(CAN_ALLOCATE_KEYS); 
  }
  
}
