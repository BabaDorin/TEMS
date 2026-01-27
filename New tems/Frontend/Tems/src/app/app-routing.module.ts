import { Routes } from '@angular/router';
import { LoginComponent } from './public/user-pages/login/login.component';
import { RegisterComponent } from './public/user-pages/register/register.component';
import { CallbackComponent } from './public/callback/callback.component';
import { FindByTemsidComponent } from './tems-components/asset/find-by-temsid/find-by-temsid.component';
import { DashboardComponent } from './public/dashboard/dashboard.component';
import { QuickAccessComponent } from './tems-components/asset/quick-access/quick-access.component';
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
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'quick-access/:type', component: QuickAccessComponent, canActivate: [AuthGuard] },
  { path: 'find/:temsId', component: FindByTemsidComponent, canActivate: [AuthGuard] },
  
  // Error Pages
  {
    path: 'error-pages',
    children: [
      { path: '404', loadComponent: () => import('./public/error-pages/error404/error404.component').then(m => m.Error404Component) },
      { path: '403', loadComponent: () => import('./public/error-pages/error403/error403.component').then(m => m.Error403Component) },
      { path: '500', loadComponent: () => import('./public/error-pages/error500/error500.component').then(m => m.Error500Component) },
    ]
  },
  
  // Asset
  {
    path: 'asset',
    children: [
      { path: '', loadComponent: () => import('./tems-components/asset/view-asset/view-asset.component').then(m => m.ViewAssetComponent), canActivate: [canManageAssetsGuard] },
      { path: 'add', loadComponent: () => import('./tems-components/asset/add-asset/add-asset.component').then(m => m.AddAssetComponent), canActivate: [canManageAssetsGuard] },
      { path: 'details/:id', loadComponent: () => import('./tems-components/asset/asset-details/asset-details.component').then(m => m.AssetDetailsComponent), canActivate: [canManageAssetsGuard] },
      { path: 'all', loadComponent: () => import('./tems-components/asset/view-asset/view-asset.component').then(m => m.ViewAssetComponent), canActivate: [canManageAssetsGuard] },
      { path: 'quick-access', loadComponent: () => import('./tems-components/asset/quick-access/quick-access.component').then(m => m.QuickAccessComponent), canActivate: [canManageAssetsGuard] },
      { path: 'allocate', loadComponent: () => import('./tems-components/asset/asset-allocation/asset-allocation.component').then(m => m.AssetAllocationComponent), canActivate: [canManageAssetsGuard] },
      { path: 'allocations', loadComponent: () => import('./tems-components/view-asset-allocations/view-asset-allocations.component').then(m => m.ViewAssetAllocationsComponent), canActivate: [canManageAssetsGuard] },
      { path: 'generate-temsid', loadComponent: () => import('./tems-components/temsid-generator/temsid-generator.component').then(m => m.TemsidGeneratorComponent), canActivate: [canManageAssetsGuard] },
    ]
  },
  
  // Rooms
  {
    path: 'rooms',
    children: [
      { path: '', loadComponent: () => import('./tems-components/room/view-rooms/view-rooms.component').then(m => m.ViewRoomsComponent), canActivate: [canManageAssetsGuard] },
      { path: 'view', loadComponent: () => import('./tems-components/room/view-rooms/view-rooms.component').then(m => m.ViewRoomsComponent), canActivate: [canManageAssetsGuard] },
      { path: 'add', loadComponent: () => import('./tems-components/room/add-room/add-room.component').then(m => m.AddRoomComponent), canActivate: [canManageAssetsGuard] },
      { path: 'map', loadComponent: () => import('./tems-components/college-map/college-map.component').then(m => m.CollegeMapComponent), canActivate: [canManageAssetsGuard] },
    ]
  },
  
  // Personnel
  {
    path: 'personnel',
    children: [
      { path: '', loadComponent: () => import('./tems-components/personnel/view-personnel/view-personnel.component').then(m => m.ViewPersonnelComponent), canActivate: [canManageAssetsGuard] },
      { path: 'all', loadComponent: () => import('./tems-components/personnel/view-personnel/view-personnel.component').then(m => m.ViewPersonnelComponent), canActivate: [canManageAssetsGuard] },
      { path: 'add', loadComponent: () => import('./tems-components/personnel/add-personnel/add-personnel.component').then(m => m.AddPersonnelComponent), canActivate: [canManageAssetsGuard] },
      { path: 'details/:id', loadComponent: () => import('./tems-components/personnel/personnel-details/personnel-details.component').then(m => m.PersonnelDetailsComponent), canActivate: [canManageAssetsGuard] },
    ]
  },
  
  
  // Issues
  {
    path: 'issues',
    children: [
      { path: '', loadComponent: () => import('./tems-components/issue/view-issues/view-issues.component').then(m => m.ViewIssuesComponent), canActivate: [canManageTicketsGuard] },
      { path: 'all', loadComponent: () => import('./tems-components/issue/view-issues/view-issues.component').then(m => m.ViewIssuesComponent), canActivate: [canManageTicketsGuard] },
      { path: 'create', loadComponent: () => import('./tems-components/issue/create-issue/create-issue.component').then(m => m.CreateIssueComponent), canActivate: [canOpenTicketsGuard] },
    ]
  },
  
  // Technical Support
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
  
  // Analytics
  {
    path: 'analytics',
    children: [
      { path: '', component: DashboardComponent }
    ]
  },
  
  // Communication
  {
    path: 'communication',
    children: [
      { path: 'announcements', loadComponent: () => import('./tems-components/communication/view-announcements/view-announcements.component').then(m => m.ViewAnnouncementsComponent) },
      { path: 'logs', loadComponent: () => import('./tems-components/communication/view-logs/view-logs.component').then(m => m.ViewLogsComponent), canActivate: [canManageAssetsGuard] },
      { path: 'sendemail', loadComponent: () => import('./tems-components/send-email/send-email.component').then(m => m.SendEmailComponent), canActivate: [canManageAssetsGuard] },
    ]
  },
  
  // Library
  {
    path: 'library',
    children: [
      { path: '', loadComponent: () => import('./tems-components/library/view-library/view-library.component').then(m => m.ViewLibraryComponent), canActivate: [canManageAssetsGuard] },
      { path: 'all', loadComponent: () => import('./tems-components/library/view-library/view-library.component').then(m => m.ViewLibraryComponent), canActivate: [canManageAssetsGuard] },
      { path: 'add', loadComponent: () => import('./tems-components/library/upload-library-item/upload-library-item.component').then(m => m.UploadLibraryItemComponent), canActivate: [canManageAssetsGuard] }
    ]
  },
  
  // Reports
  {
    path: 'reports',
    children: [
      { path: '', loadComponent: () => import('./tems-components/reports/reports.component').then(m => m.ReportsComponent), canActivate: [canManageAssetsGuard] },
      { path: 'createtemplate', loadComponent: () => import('./tems-components/reports/create-report-template/create-report-template.component').then(m => m.CreateReportTemplateComponent), canActivate: [canManageAssetsGuard] },
      { path: 'updatetemplate/:id', loadComponent: () => import('./tems-components/reports/create-report-template/create-report-template.component').then(m => m.CreateReportTemplateComponent), canActivate: [canManageAssetsGuard] }
    ]
  },
  
  // Administration
  {
    path: 'administration',
    children: [
      { path: '', loadComponent: () => import('./tems-components/admin/asset-management/asset-management.component').then(m => m.AssetManagementComponent) },
      { path: 'asset', loadComponent: () => import('./tems-components/admin/asset-management/asset-management.component').then(m => m.AssetManagementComponent) },
      { path: 'users', loadComponent: () => import('./tems-components/admin/user-management/user-management.component').then(m => m.UserManagementComponent) },
      { path: 'system-configuration', loadComponent: () => import('./tems-components/admin/system-config/system-config.component').then(m => m.SystemConfigComponent) },
      { path: 'system-logs', loadComponent: () => import('./tems-components/admin/view-system-logs/view-system-logs.component').then(m => m.ViewSystemLogsComponent) },
      { path: 'bug-reports', loadComponent: () => import('./tems-components/view-bug-reports/view-bug-reports.component').then(m => m.ViewBugReportsComponent) }
    ]
  },
  
  // Authentication
  {
    path: 'auth',
    children: [
      { path: 'login', component: LoginComponent, canActivate: [GuestGuard] },
    ]
  },
  
  // Archive
  {
    path: 'archieve',
    children: [
      { path: '', loadComponent: () => import('./tems-components/archieve/view-archieve/view-archieve.component').then(m => m.ViewArchieveComponent), canActivate: [canManageAssetsGuard] },
      { path: ':id', loadComponent: () => import('./tems-components/archieve/view-archieve/view-archieve.component').then(m => m.ViewArchieveComponent), canActivate: [canManageAssetsGuard] }
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
