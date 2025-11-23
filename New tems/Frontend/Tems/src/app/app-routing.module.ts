import { Routes } from '@angular/router';
import { LoginComponent } from './public/user-pages/login/login.component';
import { RegisterComponent } from './public/user-pages/register/register.component';
import { CallbackComponent } from './public/callback/callback.component';
import { FindByTemsidComponent } from './tems-components/equipment/find-by-temsid/find-by-temsid.component';
import { DashboardComponent } from './public/dashboard/dashboard.component';
import { QuickAccessComponent } from './tems-components/equipment/quick-access/quick-access.component';
import { HomeComponent } from './tems-components/home/home.component';
import { AuthGuard } from './guards/auth.guard';
import { CanViewEntitiesGuard } from './guards/can-view-entities.guard';
import { CanManageEntitiesGuard } from './guards/can-manage-entities.guard';
import { CanSendEmailGuard } from './guards/can-send-email.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
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
  
  // Equipment
  {
    path: 'equipment',
    children: [
      { path: '', loadComponent: () => import('./tems-components/equipment/view-equipment/view-equipment.component').then(m => m.ViewEquipmentComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'add', loadComponent: () => import('./tems-components/equipment/add-equipment/add-equipment.component').then(m => m.AddEquipmentComponent), canActivate: [CanManageEntitiesGuard] },
      { path: 'details/:id', loadComponent: () => import('./tems-components/equipment/equipment-details/equipment-details.component').then(m => m.EquipmentDetailsComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'all', loadComponent: () => import('./tems-components/equipment/view-equipment/view-equipment.component').then(m => m.ViewEquipmentComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'quick-access', loadComponent: () => import('./tems-components/equipment/quick-access/quick-access.component').then(m => m.QuickAccessComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'allocate', loadComponent: () => import('./tems-components/equipment/equipment-allocation/equipment-allocation.component').then(m => m.EquipmentAllocationComponent), canActivate: [CanManageEntitiesGuard] },
      { path: 'allocations', loadComponent: () => import('./tems-components/view-equipment-allocations/view-equipment-allocations.component').then(m => m.ViewEquipmentAllocationsComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'generate-temsid', loadComponent: () => import('./tems-components/temsid-generator/temsid-generator.component').then(m => m.TemsidGeneratorComponent), canActivate: [CanViewEntitiesGuard] },
    ]
  },
  
  // Rooms
  {
    path: 'rooms',
    children: [
      { path: '', loadComponent: () => import('./tems-components/room/view-rooms/view-rooms.component').then(m => m.ViewRoomsComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'view', loadComponent: () => import('./tems-components/room/view-rooms/view-rooms.component').then(m => m.ViewRoomsComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'add', loadComponent: () => import('./tems-components/room/add-room/add-room.component').then(m => m.AddRoomComponent), canActivate: [CanManageEntitiesGuard] },
      { path: 'map', loadComponent: () => import('./tems-components/college-map/college-map.component').then(m => m.CollegeMapComponent), canActivate: [CanViewEntitiesGuard] },
    ]
  },
  
  // Personnel
  {
    path: 'personnel',
    children: [
      { path: '', loadComponent: () => import('./tems-components/personnel/view-personnel/view-personnel.component').then(m => m.ViewPersonnelComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'all', loadComponent: () => import('./tems-components/personnel/view-personnel/view-personnel.component').then(m => m.ViewPersonnelComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'add', loadComponent: () => import('./tems-components/personnel/add-personnel/add-personnel.component').then(m => m.AddPersonnelComponent), canActivate: [CanManageEntitiesGuard] },
      { path: 'details/:id', loadComponent: () => import('./tems-components/personnel/personnel-details/personnel-details.component').then(m => m.PersonnelDetailsComponent), canActivate: [CanViewEntitiesGuard] },
    ]
  },
  
  
  // Issues
  {
    path: 'issues',
    children: [
      { path: '', loadComponent: () => import('./tems-components/issue/view-issues/view-issues.component').then(m => m.ViewIssuesComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'all', loadComponent: () => import('./tems-components/issue/view-issues/view-issues.component').then(m => m.ViewIssuesComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'create', loadComponent: () => import('./tems-components/issue/create-issue/create-issue.component').then(m => m.CreateIssueComponent) },
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
      { path: 'logs', loadComponent: () => import('./tems-components/communication/view-logs/view-logs.component').then(m => m.ViewLogsComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'sendemail', loadComponent: () => import('./tems-components/send-email/send-email.component').then(m => m.SendEmailComponent), canActivate: [CanSendEmailGuard] },
    ]
  },
  
  // Library
  {
    path: 'library',
    children: [
      { path: '', loadComponent: () => import('./tems-components/library/view-library/view-library.component').then(m => m.ViewLibraryComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'all', loadComponent: () => import('./tems-components/library/view-library/view-library.component').then(m => m.ViewLibraryComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'add', loadComponent: () => import('./tems-components/library/upload-library-item/upload-library-item.component').then(m => m.UploadLibraryItemComponent), canActivate: [CanManageEntitiesGuard] }
    ]
  },
  
  // Reports
  {
    path: 'reports',
    children: [
      { path: '', loadComponent: () => import('./tems-components/reports/reports.component').then(m => m.ReportsComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'createtemplate', loadComponent: () => import('./tems-components/reports/create-report-template/create-report-template.component').then(m => m.CreateReportTemplateComponent), canActivate: [CanManageEntitiesGuard] },
      { path: 'updatetemplate/:id', loadComponent: () => import('./tems-components/reports/create-report-template/create-report-template.component').then(m => m.CreateReportTemplateComponent), canActivate: [CanManageEntitiesGuard] }
    ]
  },
  
  // Administration
  {
    path: 'administration',
    children: [
      { path: '', loadComponent: () => import('./tems-components/admin/equipment-management/equipment-management.component').then(m => m.EquipmentManagementComponent) },
      { path: 'equipment', loadComponent: () => import('./tems-components/admin/equipment-management/equipment-management.component').then(m => m.EquipmentManagementComponent) },
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
      { path: 'login', component: LoginComponent },
    ]
  },
  
  // Archive
  {
    path: 'archieve',
    children: [
      { path: '', loadComponent: () => import('./tems-components/archieve/view-archieve/view-archieve.component').then(m => m.ViewArchieveComponent), canActivate: [CanViewEntitiesGuard] },
      { path: ':id', loadComponent: () => import('./tems-components/archieve/view-archieve/view-archieve.component').then(m => m.ViewArchieveComponent), canActivate: [CanViewEntitiesGuard] }
    ]
  },
  
  // Profile
  {
    path: 'profile',
    children: [
      { path: 'view', loadComponent: () => import('./tems-components/view-profile/view-profile.component').then(m => m.ViewProfileComponent), canActivate: [CanViewEntitiesGuard] },
      { path: 'view/:id', loadComponent: () => import('./tems-components/view-profile/view-profile.component').then(m => m.ViewProfileComponent), canActivate: [CanViewEntitiesGuard] },
    ]
  },
];
