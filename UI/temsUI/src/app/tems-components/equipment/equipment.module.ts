import { EquipmentSummaryAnalyticsModule } from './../../modules/summary-analytics/equipment-summary-analytics/equipment-summary-analytics.module';
import { ChildEquipmentContainerComponent } from './../../tems-component/equipment/child-equipment-container/child-equipment-container.component';
import { FileUploadModule } from './../../modules/file-upload/file-upload.module';
import { EquipmentDetailsGeneralComponent } from './equipment-details/equipment-details-general/equipment-details-general.component';
import { ViewDefinitionComponent } from './view-definition/view-definition.component';
import { TemsAgGridModule } from './../../modules/tems-ag-grid/tems-ag-grid.module';
import { ViewEquipmentAllocationsComponent } from './../view-equipment-allocations/view-equipment-allocations.component';
import { ViewTypeComponent } from './view-type/view-type.component';
import { TemsFormsModule } from './../../modules/tems-forms/tems-forms.module';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { AddDefinitionComponent } from './add-definition/add-definition.component';
import { AddTypeComponent } from './add-type/add-type.component';
import { ViewEquipmentComponent } from './view-equipment/view-equipment.component';
import { AddEquipmentComponent } from './add-equipment/add-equipment.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EquipmentRoutingModule } from './equipment-routing.module';
import { EquipmentDetailsComponent } from './equipment-details/equipment-details.component';
import { EquipmentDetailsLogsComponent } from './equipment-details/equipment-details-logs/equipment-details-logs.component';
import { EquipmentDetailsAllocationsComponent } from './equipment-details/equipment-details-allocations/equipment-details-allocations.component';
import { EquipmentDetailsIssuesComponent } from './equipment-details/equipment-details-issues/equipment-details-issues.component';
import { MaterialModule } from 'src/app/modules/material/material.module';
import { EntitySharedModule } from 'src/app/modules/entity-shared/entity-shared.module';
import { AnalyticsModule } from 'src/app/modules/analytics/analytics.module';
import { AddPropertyComponent } from './add-property/add-property.component';
import { ViewPropertyComponent } from './view-property/view-property.component';
import { AttachEquipmentComponent } from './attach-equipment/attach-equipment.component';
import { BulkUploadComponent } from './bulk-upload/bulk-upload.component';
import { BulkUploadResultsComponent } from './bulk-upload-results/bulk-upload-results.component';
import { EquipmentLabelComponent } from './equipment-label/equipment-label.component';
import { EquipmentSerialNumberComponent } from './equipment-serial-number/equipment-serial-number.component';

@NgModule({
  declarations: [
    EquipmentDetailsLogsComponent,
    EquipmentDetailsAllocationsComponent,
    ViewEquipmentComponent,
    AddEquipmentComponent,
    AddTypeComponent,
    AddDefinitionComponent,
    EquipmentDetailsIssuesComponent,
    AddPropertyComponent,
    ViewTypeComponent,
    ViewPropertyComponent,
    ViewDefinitionComponent,
    ViewEquipmentAllocationsComponent,
    EquipmentDetailsComponent,
    EquipmentDetailsGeneralComponent,
    AttachEquipmentComponent,
    BulkUploadComponent,
    BulkUploadResultsComponent,
    EquipmentLabelComponent,
    EquipmentSerialNumberComponent,
    ChildEquipmentContainerComponent
  ],
  imports: [
    CommonModule,
    EquipmentRoutingModule,
    ScrollingModule,
    MaterialModule,
    TemsFormsModule,
    TemsAgGridModule,
    EntitySharedModule,
    AnalyticsModule,
    FileUploadModule,
    EquipmentSummaryAnalyticsModule,
  ],
  exports: [
  ],
  entryComponents: []
})
export class EquipmentModule { }
