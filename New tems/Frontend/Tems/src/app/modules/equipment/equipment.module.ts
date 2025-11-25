import { FindByTemsidComponent } from './../../tems-components/equipment/find-by-temsid/find-by-temsid.component';
import { IncludeEquipmentLabelsModule } from './include-equipment-tags.module';
import { TemsidGeneratorComponent } from './../../tems-components/temsid-generator/temsid-generator.component';
import { AttachEquipmentAgGridModule } from '../tems-ag-grid/attach-equipment-ag-grid.module';
import { MultipleSelectionDropdownModule } from '../forms/multiple-selection-dropdown/multiple-selection-dropdown.module';
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
import { FileUploadModule } from '../file-upload/file-upload.module';
import { EquipmentSummaryAnalyticsModule } from '../summary-analytics/equipment-summary-analytics/equipment-summary-analytics.module';
import { TemsAgGridModule } from '../tems-ag-grid/tems-ag-grid.module';
import { TEMS_FORMS_IMPORTS } from '../tems-forms/tems-forms.module';
import { ViewEquipmentAllocationsComponent } from '../../tems-components/view-equipment-allocations/view-equipment-allocations.component';
import { AddDefinitionComponent } from '../../tems-components/equipment/add-definition/add-definition.component';
import { AddEquipmentComponent } from '../../tems-components/equipment/add-equipment/add-equipment.component';
import { AddPropertyComponent } from '../../tems-components/equipment/add-property/add-property.component';
import { AddTypeComponent } from '../../tems-components/equipment/add-type/add-type.component';
import { AttachEquipmentComponent } from '../../tems-components/equipment/attach-equipment/attach-equipment.component';
import { BulkUploadResultsComponent } from '../../tems-components/equipment/bulk-upload-results/bulk-upload-results.component';
import { BulkUploadComponent } from '../../tems-components/equipment/bulk-upload/bulk-upload.component';
import { ChildEquipmentContainerComponent } from '../../tems-components/equipment/child-equipment-container/child-equipment-container.component';
import { EquipmentDetailsAllocationsComponent } from '../../tems-components/equipment/equipment-details/equipment-details-allocations/equipment-details-allocations.component';
import { EquipmentDetailsGeneralComponent } from '../../tems-components/equipment/equipment-details/equipment-details-general/equipment-details-general.component';
import { EquipmentDetailsIssuesComponent } from '../../tems-components/equipment/equipment-details/equipment-details-issues/equipment-details-issues.component';
import { EquipmentDetailsLogsComponent } from '../../tems-components/equipment/equipment-details/equipment-details-logs/equipment-details-logs.component';
import { EquipmentDetailsComponent } from '../../tems-components/equipment/equipment-details/equipment-details.component';
import { EquipmentRoutingModule } from './equipment-routing.module';
import { EquipmentSerialNumberComponent } from '../../tems-components/equipment/equipment-serial-number/equipment-serial-number.component';
import { ViewDefinitionComponent } from '../../tems-components/equipment/view-definition/view-definition.component';
import { ViewEquipmentComponent } from '../../tems-components/equipment/view-equipment/view-equipment.component';
import { ViewPropertyComponent } from '../../tems-components/equipment/view-property/view-property.component';
import { ViewTypeComponent } from '../../tems-components/equipment/view-type/view-type.component';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { EquipmentLabelModule } from './equipment-label.module';

@NgModule({
  declarations: [
  ],
  imports: [
    ViewPropertyComponent,
    ViewDefinitionComponent,
    ViewEquipmentAllocationsComponent,
    EquipmentDetailsComponent,
    AttachEquipmentComponent,
    BulkUploadComponent,
    BulkUploadResultsComponent,
    EquipmentSerialNumberComponent,
    ChildEquipmentContainerComponent,
    TemsidGeneratorComponent,
    EquipmentDetailsLogsComponent,
    EquipmentDetailsAllocationsComponent,
    ViewEquipmentComponent,
    AddEquipmentComponent,
    AddTypeComponent,
    AddDefinitionComponent,
    EquipmentDetailsIssuesComponent,
    AddPropertyComponent,
    ViewTypeComponent,
    EquipmentDetailsGeneralComponent,
    CommonModule,
    EquipmentRoutingModule,
    MultipleSelectionDropdownModule,
    ScrollingModule,
    TranslateModule,
    ...TEMS_FORMS_IMPORTS,
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
    EquipmentLabelModule,
    IncludeEquipmentLabelsModule
  ],
  exports: [
  ],
})
export class EquipmentModule { }
