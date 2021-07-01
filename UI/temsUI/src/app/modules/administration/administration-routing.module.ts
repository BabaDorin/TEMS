import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EquipmentManagementComponent } from '../../tems-components/admin/equipment-management/equipment-management.component';
import { SystemConfigComponent } from '../../tems-components/admin/system-config/system-config.component';
import { UserManagementComponent } from '../../tems-components/admin/user-management/user-management.component';
import { ViewSystemLogsComponent } from '../../tems-components/admin/view-system-logs/view-system-logs.component';

const routes: Routes = [
  { path: '', component: EquipmentManagementComponent },
  { path: 'equipment', component: EquipmentManagementComponent },
  { path: 'users', component: UserManagementComponent },
  { path: 'system-configuration', component: SystemConfigComponent },
  { path: 'system-logs', component: ViewSystemLogsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdministrationRoutingModule { }
