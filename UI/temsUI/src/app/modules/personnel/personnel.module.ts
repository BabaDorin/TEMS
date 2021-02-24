import { AddPersonnelComponent } from './../../tems-components/personnel/add-personnel/add-personnel.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PersonnelRoutingModule } from './personnel-routing.module';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { PersonnelDetailsComponent } from 'src/app/tems-components/personnel/personnel-details/personnel-details.component';
import { AgGridModule } from 'ag-grid-angular';
import { MaterialModule } from '../material/material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FormlyModule } from '@ngx-formly/core';

@NgModule({
  declarations: [
    PersonnelDetailsComponent, 
    AddPersonnelComponent,
  ],

  imports: [
    CommonModule,
    PersonnelRoutingModule,
    AgGridModule,
    MaterialModule,
    FormsModule,
    FormlyModule,
    ReactiveFormsModule,

    EntitySharedModule,
  ]
})
export class PersonnelModule { }
