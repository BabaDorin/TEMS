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
import { DashboardComponent } from './reusable-components/dashboard/dashboard.component';


const routes: Routes = [
  // Equipment
  { path: 'equipment/all', component: ViewEquipmentComponent },
  { path: 'equipment/add', component: AddEquipmentComponent },
  { path: 'equipment/quick-access', component: QuickAccessComponent },
  { path: 'equipment/allocate', component: EquipmentAllocationComponent },

  // Rooms
  { path: 'rooms/all', component: ViewRoomsComponent },
  { path: 'rooms/allocate', component: RoomAllocationComponent },

  // Personnel
  { path: 'personnel/all', component: ViewPersonnelComponent },
  { path: 'personnel/add', component: AddPersonnelComponent },
  { path: 'personnel/allocate', component: PersonnelAllocationComponent },

  // Keys
  { path: 'keys/all', component: ViewKeysComponent },
  { path: 'keys/allocate', component: KeysAllocationsComponent },
  { path: 'keys/allocations', component: ViewKeysAllocationsComponent },

  // Issues
  { path: 'issues/all', component: ViewIssuesComponent },
  { path: 'issues/create', component: CreateIssueComponent },


  // Library
  { path: 'library/all', component: ViewLibraryComponent },
  
  // Administration
  { path: 'administration/equipment-management', component: EquipmentManagementComponent },
  { path: 'administration/user-management', component: UserManagementComponent },
  { path: 'administration/role-management', component: RoleManagementComponent },
  { path: 'administration/system-configuration', component: SystemConfigComponent },

  // Reports
  { path: 'reports/general', component: ReportsComponent },

  // College Map
  { path: 'college-map', component: CollegeMapComponent },

  // Communication
  { path: 'communication/announcements', component: ViewAnnouncementsComponent },
  { path: 'communication/logs', component: ViewLogsComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
