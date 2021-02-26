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
import { PersonnelDetailsLogsComponent } from 'src/app/tems-components/personnel/personnel-details-logs/personnel-details-logs.component';
import { PersonnelDetailsGeneralComponent } from 'src/app/tems-components/personnel/personnel-details-general/personnel-details-general.component';
import { PersonnelDetailsIssuesComponent } from 'src/app/tems-components/personnel/personnel-details-issues/personnel-details-issues.component';
import { PersonnelDetailsAllocationsComponent } from 'src/app/tems-components/personnel/personnel-details-allocations/personnel-details-allocations.component';
import { AnalyticsModule } from '../analytics/analytics.module';

@NgModule({
  declarations: [
    PersonnelDetailsComponent, 
    AddPersonnelComponent,
    ViewPersonnelComponent,
    AgGridPersonnelComponent,
    PersonnelDetailsLogsComponent,
    SummaryPersonnelAnalyticsComponent,
    PersonnelDetailsGeneralComponent,
    PersonnelDetailsIssuesComponent,
    PersonnelDetailsAllocationsComponent,
  ],

  imports: [
    CommonModule,
    PersonnelRoutingModule,
    AgGridModule,
    MaterialModule,
    FormsModule,
    FormlyModule,
    ReactiveFormsModule,

    // Shared modules
    EntitySharedModule,
    AnalyticsModule,
  ]
})
export class PersonnelModule { }
