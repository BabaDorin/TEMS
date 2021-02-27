import { ReportsComponent } from './../../tems-components/reports/reports.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportsRoutingModule } from './reports-routing.module';


@NgModule({
  declarations: [
    ReportsComponent    
  ],
  imports: [
    CommonModule,
    ReportsRoutingModule
  ],
  exports: [
    ReportsComponent,
  ]
})
export class ReportsModule { }
