import { EmailModule } from './../email/email/email.module';
import { EquipmentModule } from './../../tems-components/equipment/equipment.module';
import { BrowserModule } from '@angular/platform-browser';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { ViewPersonnelComponent } from 'src/app/tems-components/personnel/view-personnel/view-personnel.component';
import { AddPersonnelComponent } from './../../tems-components/personnel/add-personnel/add-personnel.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PersonnelRoutingModule } from './personnel-routing.module';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { PersonnelDetailsComponent } from 'src/app/tems-components/personnel/personnel-details/personnel-details.component';
import { MaterialModule } from '../material/material.module';
import { ReactiveFormsModule } from '@angular/forms';
import { SummaryPersonnelAnalyticsComponent } from 'src/app/tems-components/analytics/summary-personnel-analytics/summary-personnel-analytics.component';
import { PersonnelDetailsLogsComponent } from 'src/app/tems-components/personnel/personnel-details-logs/personnel-details-logs.component';
import { PersonnelDetailsGeneralComponent } from 'src/app/tems-components/personnel/personnel-details-general/personnel-details-general.component';
import { PersonnelDetailsIssuesComponent } from 'src/app/tems-components/personnel/personnel-details-issues/personnel-details-issues.component';
import { PersonnelDetailsAllocationsComponent } from 'src/app/tems-components/personnel/personnel-details-allocations/personnel-details-allocations.component';
import { AnalyticsModule } from '../analytics/analytics.module';
import { TemsAgGridModule } from '../tems-ag-grid/tems-ag-grid.module';

@NgModule({
  declarations: [
    PersonnelDetailsComponent, 
    AddPersonnelComponent,
    ViewPersonnelComponent,
    PersonnelDetailsLogsComponent,
    SummaryPersonnelAnalyticsComponent,
    PersonnelDetailsIssuesComponent,
    PersonnelDetailsAllocationsComponent,
  ],

  imports: [
    CommonModule,
    PersonnelRoutingModule,
    MaterialModule,
    TemsFormsModule,
    ReactiveFormsModule,

    EmailModule,
    // Shared modules
    EntitySharedModule,
    EquipmentModule,
    AnalyticsModule,
    TemsAgGridModule
  ]
})
export class PersonnelModule { }
