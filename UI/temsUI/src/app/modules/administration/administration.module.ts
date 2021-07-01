import { MatCheckboxModule } from '@angular/material/checkbox';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslateModule } from '@ngx-translate/core';
import { NgxPaginationModule } from 'ngx-pagination';
import { GenericContainerModule } from '../../shared/generic-container/generic-container.module';
import { EquipmentDefinitionsListComponent } from '../../tems-components/admin/equipment-management/equipment-definitions-list/equipment-definitions-list.component';
import { EquipmentManagementComponent } from '../../tems-components/admin/equipment-management/equipment-management.component';
import { EquipmentTypesListComponent } from '../../tems-components/admin/equipment-management/equipment-types-list/equipment-types-list.component';
import { ManageDefinitionsComponent } from '../../tems-components/admin/equipment-management/manage-definitions/manage-definitions.component';
import { ManagePropertiesComponent } from '../../tems-components/admin/equipment-management/manage-properties/manage-properties.component';
import { ManageTypesComponent } from '../../tems-components/admin/equipment-management/manage-types/manage-types.component';
import { PropertiesListComponent } from '../../tems-components/admin/equipment-management/properties-list/properties-list.component';
import { SystemConfigComponent } from '../../tems-components/admin/system-config/system-config.component';
import { AddUserComponent } from '../../tems-components/admin/user-management/add-user/add-user.component';
import { UserManagementComponent } from '../../tems-components/admin/user-management/user-management.component';
import { ViewUsersComponent } from '../../tems-components/admin/user-management/view-users/view-users.component';
import { ViewSystemLogsComponent } from '../../tems-components/admin/view-system-logs/view-system-logs.component';
import { EquipmentModule } from '../../tems-components/equipment/equipment.module';
import { UserContainerComponent } from '../../tems-components/identity/user-container/user-container.component';
import { ChipsAutocompleteModule } from '../chips-autocomplete/chips-autocomplete.module';
import { DialogModule } from '../dialog/dialog.module';
import { PersonnelModule } from '../personnel/personnel.module';
import { TemsFormsModule } from '../tems-forms/tems-forms.module';
import { AdministrationRoutingModule } from './administration-routing.module';



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
    TranslateModule,
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
    MatCheckboxModule,
    MatCardModule,
  ]
})
export class AdministrationModule { }
