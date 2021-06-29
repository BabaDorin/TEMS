import { MatCardModule } from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { MatInputModule } from '@angular/material/input';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule, MatLabel } from '@angular/material/form-field';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { ViewSystemLogsComponent } from '../../tems-components/admin/view-system-logs/view-system-logs.component';
import { ChipsAutocompleteModule } from '../chips-autocomplete/chips-autocomplete.module';
import { SystemConfigComponent } from '../../tems-components/admin/system-config/system-config.component';
import { MatButtonModule } from '@angular/material/button';
import { UserContainerComponent } from '../../tems-components/identity/user-container/user-container.component';
import { GenericContainerModule } from '../../shared/generic-container/generic-container.module';
import { EquipmentDefinitionsListComponent } from '../../tems-components/admin/equipment-management/equipment-definitions-list/equipment-definitions-list.component';
import { PropertiesListComponent } from '../../tems-components/admin/equipment-management/properties-list/properties-list.component';
import { EquipmentTypesListComponent } from '../../tems-components/admin/equipment-management/equipment-types-list/equipment-types-list.component';
import { PersonnelModule } from '../personnel/personnel.module';
import { EquipmentModule } from '../../tems-components/equipment/equipment.module';
import { EquipmentManagementComponent } from '../../tems-components/admin/equipment-management/equipment-management.component';
import { TemsFormsModule } from '../tems-forms/tems-forms.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdministrationRoutingModule } from './administration-routing.module';
import { UserManagementComponent } from '../../tems-components/admin/user-management/user-management.component';
import { AddUserComponent } from '../../tems-components/admin/user-management/add-user/add-user.component';
import { ViewUsersComponent } from '../../tems-components/admin/user-management/view-users/view-users.component';
import { ManageDefinitionsComponent } from '../../tems-components/admin/equipment-management/manage-definitions/manage-definitions.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { DialogModule } from '../dialog/dialog.module';
import { ManageTypesComponent } from '../../tems-components/admin/equipment-management/manage-types/manage-types.component';
import { ManagePropertiesComponent } from '../../tems-components/admin/equipment-management/manage-properties/manage-properties.component';


@NgModule({
declarations: [
    EquipmentManagementComponent,
    UserManagementComponent,
    AddUserComponent,
    ViewUsersComponent,
    ManageDefinitionsComponent,
    EquipmentTypesListComponent,
    PropertiesListComponent,
    EquipmentDefinitionsListComponent,
    ManageTypesComponent,
    ManagePropertiesComponent,
    UserContainerComponent,
    SystemConfigComponent,
    ViewSystemLogsComponent,
  ],
  imports: [
    CommonModule,
    MatButtonModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule,
    MatExpansionModule,
    GenericContainerModule,
    AdministrationRoutingModule,
    TemsFormsModule,
    EquipmentModule,
    DialogModule,
    PersonnelModule,
    NgxPaginationModule,
    MatMenuModule,
    ChipsAutocompleteModule,
    MatIconModule,
    MatCardModule,
  ]
})
export class AdministrationModule { }
