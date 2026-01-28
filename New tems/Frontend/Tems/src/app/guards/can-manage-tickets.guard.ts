import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';

export const canManageTicketsGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (tokenService.canManageTickets()) {
    return true;
  }

  router.navigate(['/unauthorized']);
  return false;
};
