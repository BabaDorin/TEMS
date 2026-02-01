import { Routes } from '@angular/router';
import { LoginComponent } from './public/user-pages/login/login.component';
import { CallbackComponent } from './public/callback/callback.component';
import { HomeComponent } from './tems-components/home/home.component';
import { AuthGuard } from './guards/auth.guard';
import { GuestGuard } from './guards/guest.guard';
import { canManageAssetsGuard } from './guards/can-manage-assets.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'callback', component: CallbackComponent },
  { path: 'login', component: LoginComponent, canActivate: [GuestGuard] },
  
  // Error Pages
  {
    path: 'error-pages',
    children: [
      { path: '404', loadComponent: () => import('./public/error-pages/error404/error404.component').then(m => m.Error404Component) },
      { path: '403', loadComponent: () => import('./public/error-pages/error403/error403.component').then(m => m.Error403Component) },
      { path: '500', loadComponent: () => import('./public/error-pages/error500/error500.component').then(m => m.Error500Component) },
    ]
  },
  
  // Assets
  {
    path: 'assets',
    children: [
      { path: '', redirectTo: 'view', pathMatch: 'full' },
      { path: 'view', loadComponent: () => import('./tems-components/asset-module/view-assets/view-assets.component').then(m => m.ViewAssetsComponent) },
      { path: 'management', loadComponent: () => import('./tems-components/asset-module/asset-management/asset-management.component').then(m => m.AssetManagementComponent) },
      { path: ':id', loadComponent: () => import('./tems-components/asset-module/asset-detail/asset-detail.component').then(m => m.AssetDetailComponent) }
    ]
  },
  
  // Locations
  {
    path: 'locations',
    children: [
      { path: '', redirectTo: 'view', pathMatch: 'full' },
      { path: 'view', loadComponent: () => import('./tems-components/location-module/view-locations/view-locations.component').then(m => m.ViewLocationsComponent), canActivate: [canManageAssetsGuard] },
      { path: ':id', loadComponent: () => import('./tems-components/location-module/room-detail/room-detail.component').then(m => m.RoomDetailComponent), canActivate: [canManageAssetsGuard] },
    ]
  },
  
  // Technical Support
  {
    path: 'technical-support',
    children: [
      { path: '', redirectTo: 'tickets', pathMatch: 'full' },
      { path: 'ticket-types', loadComponent: () => import('./tems-components/ticket-management/view-ticket-types/view-ticket-types.component').then(m => m.ViewTicketTypesComponent) },
      { path: 'tickets', loadComponent: () => import('./tems-components/ticket-management/view-tickets/view-tickets.component').then(m => m.ViewTicketsComponent) },
      { path: 'tickets/:id', loadComponent: () => import('./tems-components/ticket-management/ticket-detail/ticket-detail.component').then(m => m.TicketDetailComponent) },
    ]
  },
  
  // User Management (Administration)
  {
    path: 'administration',
    children: [
      { path: 'users', loadComponent: () => import('./tems-components/admin/user-management/user-management.component').then(m => m.UserManagementComponent) }
    ]
  },
  
  // Profile
  {
    path: 'profile',
    children: [
      { path: 'view', loadComponent: () => import('./tems-components/view-profile/view-profile.component').then(m => m.ViewProfileComponent), canActivate: [AuthGuard] },
      { path: 'view/:id', loadComponent: () => import('./tems-components/view-profile/view-profile.component').then(m => m.ViewProfileComponent), canActivate: [AuthGuard] },
    ]
  },
];
