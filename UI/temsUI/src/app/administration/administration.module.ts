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
import { AddUserComponent } from './add-user/add-user.component';
import { ViewUsersComponent } from '../tems-components/admin/user-management/view-users/view-users.component';
import { ManageTypesPropertiesComponent } from '../tems-components/admin/equipment-management/manage-types-properties/manage-types-properties.component';
import { ManageDefinitionsComponent } from '../tems-components/admin/equipment-management/manage-definitions/manage-definitions.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { DialogModule } from '../modules/dialog/dialog.module';


@NgModule({
declarations: [
    AdministrationComponent,
    EquipmentManagementComponent,
    RoleManagementComponent,
    UserManagementComponent,
    AddUserComponent,
    ViewUsersComponent,
    ManageTypesPropertiesComponent,
    ManageDefinitionsComponent
  ],
  imports: [
    CommonModule,
    AdministrationRoutingModule,
    TemsFormsModule,
    MaterialModule,
    EquipmentModule,
    DialogModule,
    PersonnelModule,
    NgxPaginationModule
  ]
})
export class AdministrationModule { }
