import { AttachEquipmentAgGridModule } from './../../modules/tems-ag-grid/attach-equipment-ag-grid.module';
import { MultipleSelectionDropdownModule } from './../../modules/forms/multiple-selection-dropdown/multiple-selection-dropdown.module';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule } from '@angular/material/dialog';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatOptionModule } from '@angular/material/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslateModule } from '@ngx-translate/core';
import { AnalyticsModule } from 'src/app/modules/analytics/analytics.module';
import { EntitySharedModule } from 'src/app/modules/entity-shared/entity-shared.module';
import { FileUploadModule } from './../../modules/file-upload/file-upload.module';
import { EquipmentSummaryAnalyticsModule } from './../../modules/summary-analytics/equipment-summary-analytics/equipment-summary-analytics.module';
import { TemsAgGridModule } from './../../modules/tems-ag-grid/tems-ag-grid.module';
import { TemsFormsModule } from './../../modules/tems-forms/tems-forms.module';
import { ViewEquipmentAllocationsComponent } from './../view-equipment-allocations/view-equipment-allocations.component';
import { AddDefinitionComponent } from './add-definition/add-definition.component';
import { AddEquipmentComponent } from './add-equipment/add-equipment.component';
import { AddPropertyComponent } from './add-property/add-property.component';
import { AddTypeComponent } from './add-type/add-type.component';
import { AttachEquipmentComponent } from './attach-equipment/attach-equipment.component';
import { BulkUploadResultsComponent } from './bulk-upload-results/bulk-upload-results.component';
import { BulkUploadComponent } from './bulk-upload/bulk-upload.component';
import { ChildEquipmentContainerComponent } from './child-equipment-container/child-equipment-container.component';
import { EquipmentDetailsAllocationsComponent } from './equipment-details/equipment-details-allocations/equipment-details-allocations.component';
import { EquipmentDetailsGeneralComponent } from './equipment-details/equipment-details-general/equipment-details-general.component';
import { EquipmentDetailsIssuesComponent } from './equipment-details/equipment-details-issues/equipment-details-issues.component';
import { EquipmentDetailsLogsComponent } from './equipment-details/equipment-details-logs/equipment-details-logs.component';
import { EquipmentDetailsComponent } from './equipment-details/equipment-details.component';
import { EquipmentLabelComponent } from './equipment-label/equipment-label.component';
import { EquipmentRoutingModule } from './equipment-routing.module';
import { EquipmentSerialNumberComponent } from './equipment-serial-number/equipment-serial-number.component';
import { ViewDefinitionComponent } from './view-definition/view-definition.component';
import { ViewEquipmentComponent } from './view-equipment/view-equipment.component';
import { ViewPropertyComponent } from './view-property/view-property.component';
import { ViewTypeComponent } from './view-type/view-type.component';
import { MatCheckboxModule } from '@angular/material/checkbox';

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
    ChildEquipmentContainerComponent,
  ],
  imports: [
    CommonModule,
    EquipmentRoutingModule,
    MultipleSelectionDropdownModule,
    ScrollingModule,
    TranslateModule,
    TemsFormsModule,
    MatDialogModule,
    TemsAgGridModule,
    MatInputModule,
    MatSelectModule,
    MatOptionModule,
    MatTabsModule,
    MatButtonModule,
    MatMenuModule,
    MatFormFieldModule,
    MatSlideToggleModule,
    MatProgressBarModule,
    EntitySharedModule,
    AnalyticsModule,
    MatExpansionModule,
    MatTooltipModule,
    FileUploadModule,
    MatIconModule,
    EquipmentSummaryAnalyticsModule,
    MatCheckboxModule,
    AttachEquipmentAgGridModule,
  ],
  exports: [
  ],
})
export class EquipmentModule { }
