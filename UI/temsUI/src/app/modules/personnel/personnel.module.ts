import { AgGridPersonnelComponent } from './../../tems-components/personnel/ag-grid-personnel/ag-grid-personnel.component';
import { ViewPersonnelComponent } from 'src/app/tems-components/personnel/view-personnel/view-personnel.component';
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
import { SummaryPersonnelAnalyticsComponent } from 'src/app/tems-components/analytics/summary-personnel-analytics/summary-personnel-analytics.component';

@NgModule({
  declarations: [
    PersonnelDetailsComponent, 
    AddPersonnelComponent,
    ViewPersonnelComponent,
    SummaryPersonnelAnalyticsComponent,
    AgGridPersonnelComponent,
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
