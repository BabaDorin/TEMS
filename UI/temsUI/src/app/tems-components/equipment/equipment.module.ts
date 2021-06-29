import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatMenuModule } from '@angular/material/menu';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { EquipmentSummaryAnalyticsModule } from './../../modules/summary-analytics/equipment-summary-analytics/equipment-summary-analytics.module';
import { ChildEquipmentContainerComponent } from './child-equipment-container/child-equipment-container.component';
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
import { EntitySharedModule } from 'src/app/modules/entity-shared/entity-shared.module';
import { AnalyticsModule } from 'src/app/modules/analytics/analytics.module';
import { AddPropertyComponent } from './add-property/add-property.component';
import { ViewPropertyComponent } from './view-property/view-property.component';
import { AttachEquipmentComponent } from './attach-equipment/attach-equipment.component';
import { BulkUploadComponent } from './bulk-upload/bulk-upload.component';
import { BulkUploadResultsComponent } from './bulk-upload-results/bulk-upload-results.component';
import { EquipmentLabelComponent } from './equipment-label/equipment-label.component';
import { EquipmentSerialNumberComponent } from './equipment-serial-number/equipment-serial-number.component';
import { MatOptionModule } from '@angular/material/core';

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
    TemsFormsModule,
    TemsAgGridModule,
    MatInputModule,
    MatSelectModule,
    MatOptionModule,
    MatTabsModule,
    MatButtonModule,
    MatMenuModule,
    MatFormFieldModule,
    MatProgressBarModule,
    EntitySharedModule,
    AnalyticsModule,
    MatExpansionModule,
    MatIconModule,
    FileUploadModule,
    MatIconModule,
    EquipmentSummaryAnalyticsModule,
  ],
  exports: [
  ],
  entryComponents: []
})
export class EquipmentModule { }
