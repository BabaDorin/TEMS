import { ManageCommunicationGuard } from './guards/communication-guards/manage-communication/manage-communication.guard';
import { ViewAnalyticsGuard } from './guards/analytics-guards/view-analytics/view-analytics.guard';
import { ViewCommunicationGuard } from './guards/communication-guards/view-communication/view-communication.guard';
import { ViewReportsGuard } from './guards/reports-guards/view-reports/view-reports.guard';
import { AdminGuard } from './guards/administration/admin.guard';
import { ViewEquipmentGuard } from './guards/equipment-guards/view-equipment/view-equipment.guard';
import { ManageEquipmentGuard } from './guards/equipment-guards/manage-equipment/manage-equipment.guard';
import { ViewLibraryGuard } from './guards/library-guards/view-library/view-library.guard';
import { CreateIssuesGuard } from './guards/issues-guards/create-issues/create-issues.guard';
import { ManageIssuesGuard } from './guards/issues-guards/manage-issues/manage-issues.guard';
import { AllocateKeysGuard } from './guards/keys-guards/allocate-keys/allocate-keys.guard';
import { ViewKeysGuard } from './guards/keys-guards/view-keys/view-keys.guard';
import { ManagePersonnelGuard } from './guards/personnel-guards/manage-personnel/manage-personnel.guard';
import { ViewPersonnelGuard } from './guards/personnel-guards/view-personnel/view-personnel.guard';
import { AllocateEquipmentGuard } from './guards/equipment-guards/allocate-equipment/allocate-equipment.guard';
import { MdiComponent } from './public/icons/mdi/mdi.component';
import { ViewLogsComponent } from './tems-components/communication/view-logs/view-logs.component';
import { ViewAnnouncementsComponent } from './tems-components/communication/view-announcements/view-announcements.component';
import { CreateIssueComponent } from './tems-components/issue/create-issue/create-issue.component';
import { ViewIssuesComponent } from './tems-components/issue/view-issues/view-issues.component';
import { EquipmentAllocationComponent } from './tems-components/equipment/equipment-allocation/equipment-allocation.component';
import { CollegeMapComponent } from './tems-components/college-map/college-map.component';
import { ReportsComponent } from './tems-components/reports/reports.component';
import { SystemConfigComponent } from './tems-components/admin/system-config/system-config.component';
import { RoleManagementComponent } from './tems-components/admin/role-management/role-management.component';
import { UserManagementComponent } from './tems-components/admin/user-management/user-management.component';
import { EquipmentManagementComponent } from './tems-components/admin/equipment-management/equipment-management.component';
import { ViewLibraryComponent } from './tems-components/library/view-library/view-library.component';
import { ViewKeysComponent } from './tems-components/keys/view-keys/view-keys.component';
import { KeysAllocationsComponent } from './tems-components/keys/keys-allocations/keys-allocations.component';
import { ViewKeysAllocationsComponent } from './tems-components/keys/view-keys-allocations/view-keys-allocations.component';
import { PersonnelAllocationComponent } from './tems-components/personnel/personnel-allocation/personnel-allocation.component';
import { AddPersonnelComponent } from './tems-components/personnel/add-personnel/add-personnel.component';
import { ViewPersonnelComponent } from './tems-components/personnel/view-personnel/view-personnel.component';
import { ViewRoomsComponent } from './tems-components/room/view-rooms/view-rooms.component';
import { RoomAllocationComponent } from './tems-components/room/room-allocation/room-allocation.component';
import { QuickAccessComponent } from './tems-components/equipment/quick-access/quick-access.component';
import { ViewEquipmentComponent } from './tems-components/equipment/view-equipment/view-equipment.component';
import { AddEquipmentComponent } from './tems-components/equipment/add-equipment/add-equipment.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './public/dashboard/dashboard.component';
import { AnalyticsComponent } from './tems-components/analytics/analytics.component';
import { IconsModule } from './public/icons/icons.module';
import { EquipmentDetailsComponent } from './tems-components/equipment/equipment-details/equipment-details.component';
import { AddLogComponent } from './tems-components/communication/add-log/add-log.component';


const routes: Routes = [
  // Kind of home page
  { path: '', component: DashboardComponent },

  // Public
  { path: 'icons', component: MdiComponent},

  // Equipment
  { path: 'view/:id', component: EquipmentDetailsComponent, canActivate: [ ViewEquipmentGuard ] },
  { path: 'equipment/all', component: ViewEquipmentComponent, canActivate: [ ViewEquipmentGuard ] },
  { path: 'equipment/add', component: AddEquipmentComponent, canActivate: [ ManageEquipmentGuard ] },
  { path: 'equipment/quick-access', component: QuickAccessComponent, canActivate: [ ViewEquipmentGuard ]},
  { path: 'equipment/allocate', component: EquipmentAllocationComponent, canActivate: [ AllocateEquipmentGuard] },

  // Rooms
  { path: 'rooms/all', component: ViewRoomsComponent, canActivate: [ ViewRoomsComponent ] },
  { path: 'rooms/allocate', component: RoomAllocationComponent,  canActivate: [ AllocateEquipmentGuard]},

  // Personnel
  { path: 'personnel/all', component: ViewPersonnelComponent, canActivate: [ ViewPersonnelGuard ] },
  { path: 'personnel/add', component: AddPersonnelComponent, canActivate: [ ManagePersonnelGuard ] },
  { path: 'personnel/allocate', component: PersonnelAllocationComponent, canActivate: [ AllocateEquipmentGuard] },

  // Keys
  { path: 'keys/all', component: ViewKeysComponent, canActivate: [ ViewKeysGuard ]},
  { path: 'keys/allocate', component: KeysAllocationsComponent, canActivate: [ AllocateKeysGuard ] },
  { path: 'keys/allocations', component: ViewKeysAllocationsComponent, canActivate: [ AllocateKeysGuard ] },

  // Issues
  { path: 'issues/all', component: ViewIssuesComponent, canActivate: [ ViewIssuesComponent ] },
  { path: 'issues/create', component: CreateIssueComponent, canActivate: [ CreateIssuesGuard ] },

  // Library
  { path: 'library/all', component: ViewLibraryComponent, canActivate: [ ViewLibraryGuard ] },
  
  // Administration
  { path: 'administration/equipment-management', component: EquipmentManagementComponent, canActivate: [ AdminGuard ] },
  { path: 'administration/user-management', component: UserManagementComponent, canActivate: [ AdminGuard ] },
  { path: 'administration/role-management', component: RoleManagementComponent, canActivate: [ AdminGuard ] },
  { path: 'administration/system-configuration', component: SystemConfigComponent, canActivate: [ AdminGuard ] },

  // Reports
  { path: 'reports/general', component: ReportsComponent, canActivate: [ ViewReportsGuard ]  },

  // College Map
  { path: 'college-map', component: CollegeMapComponent, canActivate: [ ViewRoomsComponent ] },

  // Communication
  { path: 'communication/announcements', component: ViewAnnouncementsComponent, canActivate: [ ViewCommunicationGuard ] },
  { path: 'communication/logs', component: ViewLogsComponent, canActivate: [ ViewCommunicationGuard ] },
  { path: 'communication/logs/add', component: AddLogComponent, canActivate: [ ManageCommunicationGuard ] },

  // Analytics
  { path: 'analytics', component:  AnalyticsComponent, canActivate: [ ViewAnalyticsGuard ]},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
