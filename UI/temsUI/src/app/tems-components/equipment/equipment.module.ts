import { ViewTypeComponent } from './view-type/view-type.component';
import { TemsFormsModule } from './../../modules/tems-forms/tems-forms.module';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { AddDefinitionComponent } from './add-definition/add-definition.component';
import { AddTypeComponent } from './add-type/add-type.component';
import { ViewEquipmentComponent } from './view-equipment/view-equipment.component';
import { SummaryEquipmentAnalyticsComponent } from './../analytics/summary-equipment-analytics/summary-equipment-analytics.component';
import { AgGridEquipmentComponent } from './ag-grid-equipment/ag-grid-equipment.component';
import { AddEquipmentComponent } from './add-equipment/add-equipment.component';
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
import { AnalyticsModule } from 'src/app/modules/analytics/analytics.module';
import { AddPropertyComponent } from './add-property/add-property.component';
import { AgGridTooltipComponent } from 'src/app/public/ag-grid/ag-grid-tooltip/ag-grid-tooltip.component';
import { ViewPropertyComponent } from './view-property/view-property.component';
import { AgGridModule } from 'ag-grid-angular';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';

@NgModule({
  declarations: [
    EquipmentDetailsComponent,
    EquipmentDetailsGeneralComponent,
    EquipmentDetailsLogsComponent,
    EquipmentDetailsAllocationsComponent,
    ViewEquipmentComponent,
    AddEquipmentComponent,
    AgGridEquipmentComponent,
    AddTypeComponent,
    AddDefinitionComponent,
    SummaryEquipmentAnalyticsComponent,
    EquipmentDetailsIssuesComponent,
    AddPropertyComponent,
    AgGridTooltipComponent,
    ViewTypeComponent,
    ViewPropertyComponent,
  ],
  imports: [
    CommonModule,
    EquipmentRoutingModule,
    ScrollingModule,
    MaterialModule,
    TemsFormsModule,
    EntitySharedModule,
    AnalyticsModule,
  ],
  exports: [
  ],
  entryComponents: []
})
export class EquipmentModule { }
