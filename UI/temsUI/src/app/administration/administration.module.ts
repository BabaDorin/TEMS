import { PersonnelModule } from './../modules/personnel/personnel.module';
import { EquipmentModule } from './../tems-components/equipment/equipment.module';
import { RoleManagementComponent } from './../tems-components/admin/role-management/role-management.component';
import { EquipmentManagementComponent } from './../tems-components/admin/equipment-management/equipment-management.component';
import { MaterialModule } from 'src/app/modules/material/material.module';
import { TemsFormsModule } from './../modules/tems-forms/tems-forms.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdministrationRoutingModule } from './administration-routing.module';
import { AdministrationComponent } from './administration.component';
import { UserManagementComponent } from '../tems-components/admin/user-management/user-management.component';


@NgModule({
  declarations: [
    AdministrationComponent,
    EquipmentManagementComponent,
    RoleManagementComponent,
    UserManagementComponent,
  ],
  imports: [
    CommonModule,
    AdministrationRoutingModule,
    TemsFormsModule,
    MaterialModule,
    EquipmentModule,
    PersonnelModule,
  ]
})
export class AdministrationModule { }
