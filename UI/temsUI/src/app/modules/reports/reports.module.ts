import { CheckboxGroupComponent } from './../../shared/forms/checkbox-group/checkbox-group.component';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { CreateReportTemplateComponent } from './../../tems-components/reports/create-report-template/create-report-template.component';
import { MaterialModule } from 'src/app/modules/material/material.module';
import { ReportsComponent } from './../../tems-components/reports/reports.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportsRoutingModule } from './reports-routing.module';
import { ReportContainerComponent } from 'src/app/tems-components/reports/report-container/report-container.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';



@NgModule({
  declarations: [
    ReportsComponent,
    ReportContainerComponent,
    CreateReportTemplateComponent,
    CheckboxGroupComponent
  ],
  imports: [
    CommonModule,
    ReportsRoutingModule,
    MaterialModule,
    // ReactiveFormsModule,
    // FormsModule,
    TemsFormsModule
  ],
  exports: [
    ReportsComponent,
  ]
})
export class ReportsModule { }
