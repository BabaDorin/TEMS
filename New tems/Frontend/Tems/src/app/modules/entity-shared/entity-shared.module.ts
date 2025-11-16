import { IncludeEquipmentLabelsModule } from './../equipment/include-equipment-tags.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatOptionModule } from '@angular/material/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { NgxPaginationModule } from 'ngx-pagination';
import { AnalyticsModule } from 'src/app/modules/analytics/analytics.module';
import { SummaryIssuesAnalyticsComponent } from 'src/app/tems-components/analytics/summary-issues-analytics/summary-issues-analytics.component';
import { AddLogComponent } from 'src/app/tems-components/communication/add-log/add-log.component';
import { EntityAllocationsListComponent } from 'src/app/tems-components/entity-allocations-list/entity-allocations-list.component';
import { EntityLogsListComponent } from 'src/app/tems-components/entity-logs-list/entity-logs-list.component';
import { CreateIssueComponent } from 'src/app/tems-components/issue/create-issue/create-issue.component';
import { LogContainerComponent } from '../../tems-components/communication/log-container/log-container.component';
import { IssueContainerModule } from '../issues/issue-container/issue-container.module';
import { PropertyRenderComponent } from './../../public/property-render/property-render.component';
import { EntityIssuesListComponent } from './../../tems-components/entity-issues-list/entity-issues-list.component';
import { EquipmentAllocationContainerComponent } from './../../tems-components/equipment/equipment-allocation-container/equipment-allocation-container.component';
import { EquipmentAllocationComponent } from './../../tems-components/equipment/equipment-allocation/equipment-allocation.component';
import { PersonnelDetailsGeneralComponent } from './../../tems-components/personnel/personnel-details-general/personnel-details-general.component';
import { RoomDetailsGeneralComponent } from './../../tems-components/room/room-details-general/room-details-general.component';
import { ChipsAutocompleteModule } from './../chips-autocomplete/chips-autocomplete.module';
import { LoadingplaceholderModule } from './../loadingplaceholder/loadingplaceholder.module';
import { TemsAgGridModule } from './../tems-ag-grid/tems-ag-grid.module';
import { TEMS_FORMS_IMPORTS } from './../tems-forms/tems-forms.module';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    PropertyRenderComponent,
    AddLogComponent,
    CreateIssueComponent,
    EquipmentAllocationComponent,
    EntityLogsListComponent,
    EntityIssuesListComponent,
    SummaryIssuesAnalyticsComponent,
    EntityAllocationsListComponent,
    RoomDetailsGeneralComponent,
    PersonnelDetailsGeneralComponent,
    EquipmentAllocationContainerComponent,
    LogContainerComponent,
    RouterModule,
    ...TEMS_FORMS_IMPORTS,
    NgxPaginationModule,
    TemsAgGridModule,
    ReactiveFormsModule,
    AnalyticsModule,
    LoadingplaceholderModule,
    IssueContainerModule,
    ChipsAutocompleteModule,
    MatTooltipModule,
    MatIconModule,
    MatMenuModule,
    MatSlideToggleModule,
    MatExpansionModule,
    MatButtonModule,
    MatSelectModule,
    MatInputModule,
    MatOptionModule,
    MatIconModule,
    MatCardModule,
    TranslateModule,
    MatFormFieldModule,
    IncludeEquipmentLabelsModule,

    MatDialogModule,
  ],
  exports: [
    TemsAgGridModule,
    PropertyRenderComponent,
    AddLogComponent,
    CreateIssueComponent,
    EquipmentAllocationComponent,
    EntityLogsListComponent,
    EntityIssuesListComponent,
    SummaryIssuesAnalyticsComponent,
    EntityAllocationsListComponent,
    IssueContainerModule,
    PersonnelDetailsGeneralComponent,
    RoomDetailsGeneralComponent,
  ]
})
export class EntitySharedModule { }
