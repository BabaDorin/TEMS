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
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';
import { AgGridModule } from 'ag-grid-angular';

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
    BtnCellRendererComponent
  ],
  imports: [
    CommonModule,
    MaterialModule,
    RouterModule,
    TemsFormsModule,
    NgxPaginationModule,
    AgGridModule.withComponents([BtnCellRendererComponent]),
  ],
  exports: [
    ImageCarouselComponent,
    PropertyRenderComponent,
    AddLogComponent,
    CreateIssueComponent,
    EquipmentAllocationComponent,
    EntityLogsListComponent,
    EntityIssuesListComponent,
    SummaryIssuesAnalyticsComponent,
    EntityAllocationsListComponent,
    AgGridModule
  ]
})
export class EntitySharedModule { }
