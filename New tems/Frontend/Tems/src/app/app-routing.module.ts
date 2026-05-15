import { Routes } from '@angular/router';
import { LoginComponent } from './public/user-pages/login/login.component';
import { CallbackComponent } from './public/callback/callback.component';
import { HomeComponent } from './tems-components/home/home.component';
import { AuthGuard } from './guards/auth.guard';
import { GuestGuard } from './guards/guest.guard';
import { canManageAssetsGuard } from './guards/can-manage-assets.guard';
import { canManageTicketsGuard } from './guards/can-manage-tickets.guard';
import { canOpenTicketsGuard } from './guards/can-open-tickets.guard';

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
    canActivate: [canManageAssetsGuard],
    children: [
      { path: '', redirectTo: 'view', pathMatch: 'full' },
      { path: 'add', loadComponent: () => import('./tems-components/asset/add-asset/add-asset.component').then(m => m.AddAssetComponent), canActivate: [canManageAssetsGuard] },
      { path: 'view', loadComponent: () => import('./tems-components/asset-module/view-assets/view-assets.component').then(m => m.ViewAssetsComponent), canActivate: [canManageAssetsGuard] },
      { path: 'purchase-orders', loadComponent: () => import('./tems-components/asset-module/view-purchase-orders/view-purchase-orders.component').then(m => m.ViewPurchaseOrdersComponent), canActivate: [canManageAssetsGuard] },
      { path: 'management', loadComponent: () => import('./tems-components/asset-module/asset-management/asset-management.component').then(m => m.AssetManagementComponent), canActivate: [canManageAssetsGuard] },
      { path: ':id', loadComponent: () => import('./tems-components/asset-module/asset-detail/asset-detail.component').then(m => m.AssetDetailComponent), canActivate: [AuthGuard] }
    ]
  },

  {
    path: 'asset',
    children: [
      { path: 'add', redirectTo: '/assets/add', pathMatch: 'full' }
    ]
  },
  
  // Locations
  {
    path: 'locations',
    children: [
      { path: '', redirectTo: 'view', pathMatch: 'full' },
      { path: 'view', loadComponent: () => import('./tems-components/location-module/view-locations/view-locations.component').then(m => m.ViewLocationsComponent), canActivate: [canManageAssetsGuard] },
      { path: ':id', loadComponent: () => import('./tems-components/location-module/room-detail/room-detail.component').then(m => m.RoomDetailComponent), canActivate: [AuthGuard] },
    ]
  },
  
  // Technical Support
  {
    path: 'technical-support',
    children: [
      { path: '', redirectTo: 'tickets', pathMatch: 'full' },
      { path: 'ai-support', loadComponent: () => import('./tems-components/technical-support/ai-support/ai-support.component').then(m => m.AiSupportComponent), canActivate: [canOpenTicketsGuard] },
      { path: 'ticket-types', loadComponent: () => import('./tems-components/ticket-management/view-ticket-types/view-ticket-types.component').then(m => m.ViewTicketTypesComponent), canActivate: [canManageTicketsGuard] },
      { path: 'tickets', loadComponent: () => import('./tems-components/ticket-management/view-tickets/view-tickets.component').then(m => m.ViewTicketsComponent), canActivate: [canOpenTicketsGuard] },
      { path: 'tickets/:id', loadComponent: () => import('./tems-components/ticket-management/ticket-detail/ticket-detail.component').then(m => m.TicketDetailComponent), canActivate: [canOpenTicketsGuard] },
    ]
  },
  
  // User Management (Administration)
  {
    path: 'administration',
    children: [
      { path: 'users', loadComponent: () => import('./tems-components/admin/user-management/user-management.component').then(m => m.UserManagementComponent) }
    ]
  },

  // Users (non-admin routes)
  {
    path: 'users',
    children: [
      { path: 'me', loadComponent: () => import('./tems-components/user-module/user-detail/user-detail.component').then(m => m.UserDetailComponent), canActivate: [AuthGuard] },
      { path: ':id', loadComponent: () => import('./tems-components/user-module/user-detail/user-detail.component').then(m => m.UserDetailComponent), canActivate: [AuthGuard] }
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
