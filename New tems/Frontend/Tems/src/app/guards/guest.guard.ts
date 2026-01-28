import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class GuestGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    // Give a small delay to ensure auth state is loaded
    return new Promise((resolve) => {
      setTimeout(() => {
        const isLoggedIn = this.authService.isLoggedIn();
        console.log('[GuestGuard] User logged in:', isLoggedIn);
        if (isLoggedIn) {
          console.log('[GuestGuard] Redirecting authenticated user to /home');
          resolve(this.router.createUrlTree(['/home']));
        } else {
          resolve(true);
        }
      }, 50);
    });
  }
}
