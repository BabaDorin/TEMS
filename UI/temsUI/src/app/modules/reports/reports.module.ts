import { ViewTemplateComponent } from './../../tems-components/reports/view-template/view-template.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { CreateReportTemplateComponent } from '../../tems-components/reports/create-report-template/create-report-template.component';
import { MaterialModule } from 'src/app/modules/material/material.module';
import { ReportsComponent } from './../../tems-components/reports/reports.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportsRoutingModule } from './reports-routing.module';
import { ReportContainerComponent } from 'src/app/tems-components/reports/report-container/report-container.component';

@NgModule({
  declarations: [
    ReportsComponent,
    ReportContainerComponent,
    CreateReportTemplateComponent,
    ViewTemplateComponent,
  ],
  imports: [
    CommonModule,
    ReportsRoutingModule,
    MaterialModule,
    TemsFormsModule,
    NgxPaginationModule,
  ],
  exports: [
    ReportsComponent,
  ]
})
export class ReportsModule { }
