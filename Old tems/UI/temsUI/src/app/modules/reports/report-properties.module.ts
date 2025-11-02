import { CheckboxGroupModule } from './../checkbox-group/checkbox-group.module';
import { ReportPropertiesComponent } from './../../tems-components/reports/report-properties/report-properties.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    ReportPropertiesComponent
  ],
  imports: [
    CommonModule,
    CheckboxGroupModule
  ],
  exports: [
    ReportPropertiesComponent
  ]
})
export class ReportPropertiesModule { }
