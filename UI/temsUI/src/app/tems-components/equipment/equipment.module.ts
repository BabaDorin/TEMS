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
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { EquipmentService } from './../../services/equipment-service/equipment.service';
import { MatSelectModule } from '@angular/material/select';
import { ViewEquipmentComponent } from './view-equipment/view-equipment.component';
import { SummaryEquipmentAnalyticsComponent } from './../analytics/summary-equipment-analytics/summary-equipment-analytics.component';
import { AgGridEquipmentComponent } from './ag-grid-equipment/ag-grid-equipment.component';
import { AgGridModule } from 'ag-grid-angular';
import { AddEquipmentComponent } from './add-equipment/add-equipment.component';
import { PropertyRenderComponent } from './../../public/property-render/property-render.component';
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
import { ImageCarouselComponent } from 'src/app/public/image-carousel/image-carousel.component';
import { MatOptionModule } from '@angular/material/core';
import { AddLogComponent } from '../communication/add-log/add-log.component';
import { MatCheckboxModule } from '@angular/material/checkbox/public-api';
import { MatDialogModule } from '@angular/material/dialog';


@NgModule({
  declarations: [
    EquipmentDetailsComponent,
    EquipmentDetailsGeneralComponent,
    EquipmentDetailsLogsComponent,
    EquipmentDetailsAllocationsComponent,
    EquipmentAllocationComponent,
    ViewEquipmentComponent,
    PropertyRenderComponent,
    AddEquipmentComponent,
    ImageCarouselComponent,
    InputTooltipComponent,
    SelectTooltipComponent,
    AgGridEquipmentComponent,
    SummaryEquipmentAnalyticsComponent,
    ChipsAutocompleteComponent,
    AddLogComponent,
    AddTypeComponent,
    AddDefinitionComponent,
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

    MatTabsModule,
    MatExpansionModule,
    MatSelectModule,
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

    EquipmentService,
    FormlyParserService,
  ],
  entryComponents: [AddTypeComponent]
})
export class EquipmentModule { }
