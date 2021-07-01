import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslateModule } from '@ngx-translate/core';
import { SummaryPersonnelAnalyticsComponent } from 'src/app/tems-components/analytics/summary-personnel-analytics/summary-personnel-analytics.component';
import { PersonnelDetailsAllocationsComponent } from 'src/app/tems-components/personnel/personnel-details-allocations/personnel-details-allocations.component';
import { PersonnelDetailsIssuesComponent } from 'src/app/tems-components/personnel/personnel-details-issues/personnel-details-issues.component';
import { PersonnelDetailsLogsComponent } from 'src/app/tems-components/personnel/personnel-details-logs/personnel-details-logs.component';
import { PersonnelDetailsComponent } from 'src/app/tems-components/personnel/personnel-details/personnel-details.component';
import { ViewPersonnelComponent } from 'src/app/tems-components/personnel/view-personnel/view-personnel.component';
import { AnalyticsModule } from '../analytics/analytics.module';
import { EntitySharedModule } from '../entity-shared/entity-shared.module';
import { TemsAgGridModule } from '../tems-ag-grid/tems-ag-grid.module';
import { EquipmentModule } from './../../tems-components/equipment/equipment.module';
import { AddPersonnelComponent } from './../../tems-components/personnel/add-personnel/add-personnel.component';
import { EmailModule } from './../email/email/email.module';
import { EquipmentSummaryAnalyticsModule } from './../summary-analytics/equipment-summary-analytics/equipment-summary-analytics.module';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { PersonnelRoutingModule } from './personnel-routing.module';


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
    TemsFormsModule,
    ReactiveFormsModule,
    MatIconModule,
    MatMenuModule,
    MatTabsModule,
    MatBadgeModule,
    TranslateModule,
    EquipmentSummaryAnalyticsModule,
    EmailModule,
    // Shared modules
    EntitySharedModule,
    EquipmentModule,
    AnalyticsModule,
    MatButtonModule,
    TemsAgGridModule
  ]
})
export class PersonnelModule { }
