import { MatFormFieldModule } from '@angular/material/form-field';
import { TemsFormsModule } from './../../modules/tems-forms/tems-forms.module';
import { ViewIssuesComponent } from './../issue/view-issues/view-issues.component';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { AddDefinitionComponent } from './add-definition/add-definition.component';
import { AddTypeComponent } from './add-type/add-type.component';
import { SelectTooltipComponent } from './../../public/formly/select-tooltip/select-tooltip.component';
import { InputTooltipComponent } from './../../public/formly/input-tooltip/input-tooltip.component';
import { ViewEquipmentComponent } from './view-equipment/view-equipment.component';
import { SummaryEquipmentAnalyticsComponent } from './../analytics/summary-equipment-analytics/summary-equipment-analytics.component';
import { AgGridEquipmentComponent } from './ag-grid-equipment/ag-grid-equipment.component';
import { AgGridModule } from 'ag-grid-angular';
import { AddEquipmentComponent } from './add-equipment/add-equipment.component';
import { FormlyModule } from '@ngx-formly/core';
import { FormsModule, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EquipmentRoutingModule } from './equipment-routing.module';
import { EquipmentDetailsComponent } from './equipment-details/equipment-details.component';
import { EquipmentDetailsGeneralComponent } from './equipment-details/equipment-details-general/equipment-details-general.component';
import { EquipmentDetailsLogsComponent } from './equipment-details/equipment-details-logs/equipment-details-logs.component';
import { EquipmentDetailsAllocationsComponent } from './equipment-details/equipment-details-allocations/equipment-details-allocations.component';
import { EquipmentDetailsIssuesComponent } from './equipment-details/equipment-details-issues/equipment-details-issues.component';
import { MaterialModule } from 'src/app/modules/material/material.module';
import { EntitySharedModule } from 'src/app/modules/entity-shared/entity-shared.module';
import { EntityLogsListComponent } from '../entity-logs-list/entity-logs-list.component';

@NgModule({
  declarations: [
    EquipmentDetailsComponent,
    EquipmentDetailsGeneralComponent,
    EquipmentDetailsLogsComponent,
    EquipmentDetailsAllocationsComponent,
    ViewEquipmentComponent,
    AddEquipmentComponent,
    AgGridEquipmentComponent,
    SummaryEquipmentAnalyticsComponent,
    AddTypeComponent,
    AddDefinitionComponent,
    ViewIssuesComponent,
    EquipmentDetailsIssuesComponent,
  ],
  imports: [
    CommonModule,
    EquipmentRoutingModule,
    AgGridModule,
    ScrollingModule,
    MaterialModule,
    TemsFormsModule,
    // Shared modules
    EntitySharedModule
  ],
  exports: [
  ],
  entryComponents: []
})
export class EquipmentModule { }
