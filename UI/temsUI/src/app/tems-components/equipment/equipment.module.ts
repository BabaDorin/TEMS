import { MatIconModule } from '@angular/material/icon';
import { ViewIssuesComponent } from './../issue/view-issues/view-issues.component';
import { CreateIssueComponent } from './../issue/create-issue/create-issue.component';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { MatButtonModule } from '@angular/material/button';
import { AddDefinitionComponent } from './add-definition/add-definition.component';
import { AddTypeComponent } from './add-type/add-type.component';
import { SelectTooltipComponent } from './../../public/formly/select-tooltip/select-tooltip.component';
import { InputTooltipComponent } from './../../public/formly/input-tooltip/input-tooltip.component';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { EquipmentAllocationComponent } from './equipment-allocation/equipment-allocation.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { EquipmentService } from './../../services/equipment-service/equipment.service';
import { MatSelectModule } from '@angular/material/select';
import { ViewEquipmentComponent } from './view-equipment/view-equipment.component';
import { SummaryEquipmentAnalyticsComponent } from './../analytics/summary-equipment-analytics/summary-equipment-analytics.component';
import { AgGridEquipmentComponent } from './ag-grid-equipment/ag-grid-equipment.component';
import { AgGridModule } from 'ag-grid-angular';
import { AddEquipmentComponent } from './add-equipment/add-equipment.component';
import { FormlyModule } from '@ngx-formly/core';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatCardModule } from '@angular/material/card';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EquipmentRoutingModule } from './equipment-routing.module';
import { EquipmentDetailsComponent } from './equipment-details/equipment-details.component';
import { EquipmentDetailsGeneralComponent } from './equipment-details/equipment-details-general/equipment-details-general.component';
import { EquipmentDetailsLogsComponent } from './equipment-details/equipment-details-logs/equipment-details-logs.component';
import { EquipmentDetailsAllocationsComponent } from './equipment-details/equipment-details-allocations/equipment-details-allocations.component';
import { MatTabsModule } from '@angular/material/tabs';
import { MatOptionModule } from '@angular/material/core';
import { MatDialogModule } from '@angular/material/dialog';
import { SummaryEquipmentIssueAnalyticsComponent } from '../analytics/summary-equipment-issue-analytics/summary-equipment-issue-analytics.component';
import { EquipmentDetailsIssuesComponent } from './equipment-details/equipment-details-issues/equipment-details-issues.component';
import { MatRadioModule } from '@angular/material/radio';
import { EntitySharedModuleModule } from '../entity-shared-module/entity-shared-module.module';

@NgModule({
  declarations: [
    EquipmentDetailsComponent,
    EquipmentDetailsGeneralComponent,
    EquipmentDetailsLogsComponent,
    EquipmentDetailsAllocationsComponent,
    EquipmentAllocationComponent,
    ViewEquipmentComponent,
    AddEquipmentComponent,
    InputTooltipComponent,
    SelectTooltipComponent,
    AgGridEquipmentComponent,
    SummaryEquipmentAnalyticsComponent,
    AddTypeComponent,
    CreateIssueComponent,
    AddDefinitionComponent,
    SummaryEquipmentIssueAnalyticsComponent,
    ViewIssuesComponent,
    EquipmentDetailsIssuesComponent,
  ],
  imports: [
    CommonModule,
    EquipmentRoutingModule,
    FormsModule,
    MatInputModule,
    FormsModule,
    FormlyModule,
    AgGridModule,
    ReactiveFormsModule,
    ScrollingModule,
    MatIconModule,
    MatTabsModule,
    MatExpansionModule,
    MatSelectModule,
    EntitySharedModuleModule
  ],
  exports: [
    MatFormFieldModule, 
    MatButtonModule,
    MatTooltipModule,
    MatAutocompleteModule,
    // MatCheckboxModule,
    MatChipsModule,
    MatDialogModule,
    MatOptionModule,
    MatCardModule,
    MatRadioModule,
    EquipmentService,
    FormlyParserService,
    
  ],
  entryComponents: [AddTypeComponent]
})
export class EquipmentModule { }
