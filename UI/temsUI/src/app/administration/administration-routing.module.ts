import { SystemConfigComponent } from './../tems-components/admin/system-config/system-config.component';
import { RoleManagementComponent } from './../tems-components/admin/role-management/role-management.component';
import { EquipmentManagementComponent } from './../tems-components/admin/equipment-management/equipment-management.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdministrationComponent } from './administration.component';
import { UserManagementComponent } from '../tems-components/admin/user-management/user-management.component';

const routes: Routes = [
  { path: '', component: EquipmentManagementComponent },
  { path: 'equipment', component: EquipmentManagementComponent },
  { path: 'users', component: UserManagementComponent },
  { path: 'roles', component: RoleManagementComponent },
  { path: 'system-configuration', component: SystemConfigComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdministrationRoutingModule { }
