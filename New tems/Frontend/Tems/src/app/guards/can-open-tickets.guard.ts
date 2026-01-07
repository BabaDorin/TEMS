import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';

export const canOpenTicketsGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (tokenService.canOpenTickets()) {
    return true;
  }

  router.navigate(['/unauthorized']);
  return false;
};
