import { MaterialModule } from 'src/app/modules/material/material.module';
import { ReportsComponent } from './../../tems-components/reports/reports.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportsRoutingModule } from './reports-routing.module';
import { ReportContainerComponent } from 'src/app/tems-components/reports/report-container/report-container.component';


@NgModule({
  declarations: [
    ReportsComponent,
    ReportContainerComponent    
  ],
  imports: [
    CommonModule,
    ReportsRoutingModule,
    MaterialModule,
  ],
  exports: [
    ReportsComponent,
  ]
})
export class ReportsModule { }
