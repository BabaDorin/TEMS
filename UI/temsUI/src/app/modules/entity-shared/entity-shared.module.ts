import { TranslateModule } from '@ngx-translate/core';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ChipsAutocompleteModule } from './../chips-autocomplete/chips-autocomplete.module';
import { LogContainerComponent } from '../../tems-components/communication/log-container/log-container.component';
import { EquipmentAllocationContainerComponent } from './../../tems-components/equipment/equipment-allocation-container/equipment-allocation-container.component';
import { AnalyticsModule } from 'src/app/modules/analytics/analytics.module';
import { LoadingplaceholderModule } from './../loadingplaceholder/loadingplaceholder.module';
import { PersonnelDetailsGeneralComponent } from './../../tems-components/personnel/personnel-details-general/personnel-details-general.component';
import { RoomDetailsGeneralComponent } from './../../tems-components/room/room-details-general/room-details-general.component';
import { TemsAgGridModule } from './../tems-ag-grid/tems-ag-grid.module';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { EntityIssuesListComponent } from './../../tems-components/entity-issues-list/entity-issues-list.component';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { EquipmentAllocationComponent } from './../../tems-components/equipment/equipment-allocation/equipment-allocation.component';
import { PropertyRenderComponent } from './../../public/property-render/property-render.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddLogComponent } from 'src/app/tems-components/communication/add-log/add-log.component';
import { CreateIssueComponent } from 'src/app/tems-components/issue/create-issue/create-issue.component';
import { EntityLogsListComponent } from 'src/app/tems-components/entity-logs-list/entity-logs-list.component';
import { SummaryIssuesAnalyticsComponent } from 'src/app/tems-components/analytics/summary-issues-analytics/summary-issues-analytics.component';
import { EntityAllocationsListComponent } from 'src/app/tems-components/entity-allocations-list/entity-allocations-list.component';
import { IssueContainerModule } from '../issues/issue-container/issue-container.module';
import { MatOptionModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';

@NgModule({
  declarations: [
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
    LogContainerComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    TemsFormsModule,
    NgxPaginationModule,
    TemsAgGridModule,
    AnalyticsModule,
    LoadingplaceholderModule,
    IssueContainerModule,
    ChipsAutocompleteModule,
    MatTooltipModule,
    MatIconModule,
    MatMenuModule,
    MatSlideToggleModule,
    MatExpansionModule,
    MatInputModule,
    MatOptionModule,
    MatFormFieldModule,
    MatIconModule,
    MatCardModule,
    TranslateModule,
    MatFormFieldModule,
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
