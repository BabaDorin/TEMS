import { TemsAgGridModule } from './../tems-ag-grid/tems-ag-grid.module';
import { LoadingPlaceholderComponent } from './../../tems-components/loading-placeholder/loading-placeholder.component';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { EntityIssuesListComponent } from './../../tems-components/entity-issues-list/entity-issues-list.component';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { MaterialModule } from 'src/app/modules/material/material.module';
import { EquipmentAllocationComponent } from './../../tems-components/equipment/equipment-allocation/equipment-allocation.component';
import { PropertyRenderComponent } from './../../public/property-render/property-render.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ImageCarouselComponent } from 'src/app/public/image-carousel/image-carousel.component';
import { AddLogComponent } from 'src/app/tems-components/communication/add-log/add-log.component';
import { CreateIssueComponent } from 'src/app/tems-components/issue/create-issue/create-issue.component';
import { EntityLogsListComponent } from 'src/app/tems-components/entity-logs-list/entity-logs-list.component';
import { SummaryIssuesAnalyticsComponent } from 'src/app/tems-components/analytics/summary-issues-analytics/summary-issues-analytics.component';
import { EntityAllocationsListComponent } from 'src/app/tems-components/entity-allocations-list/entity-allocations-list.component';
import { IssueContainerComponent } from 'src/app/tems-components/issues/issue-container/issue-container.component';

@NgModule({
  declarations: [
    ImageCarouselComponent,
    PropertyRenderComponent,
    AddLogComponent,
    CreateIssueComponent,
    EquipmentAllocationComponent,
    EntityLogsListComponent,
    EntityIssuesListComponent,
    SummaryIssuesAnalyticsComponent,
    EntityAllocationsListComponent,
    LoadingPlaceholderComponent,
    IssueContainerComponent,
  ],
  imports: [
    CommonModule,
    MaterialModule,
    RouterModule,
    TemsFormsModule,
    NgxPaginationModule,
    TemsAgGridModule,
  ],
  exports: [
    TemsAgGridModule,
    ImageCarouselComponent,
    PropertyRenderComponent,
    AddLogComponent,
    CreateIssueComponent,
    EquipmentAllocationComponent,
    EntityLogsListComponent,
    EntityIssuesListComponent,
    SummaryIssuesAnalyticsComponent,
    EntityAllocationsListComponent,
    LoadingPlaceholderComponent,
    IssueContainerComponent,
  ]
})
export class EntitySharedModule { }
