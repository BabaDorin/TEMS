import { ClaimService } from './../services/claim.service';
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CanViewKeysGuard implements CanActivate {
  constructor(
    private claims: ClaimService
  ){

  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    return this.claims.canAllocateKeys || this.claims.canView || this.claims.canManageSystemConfiguration; 
  }
  
}
