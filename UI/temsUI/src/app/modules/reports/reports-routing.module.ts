import { CreateReportTemplateComponent } from './../../tems-components/reports/create-report-template/create-report-template.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReportsComponent } from 'src/app/tems-components/reports/reports.component';

const routes: Routes = [
  { path: '', component: ReportsComponent },
  { path: 'createTemplate', component: CreateReportTemplateComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportsRoutingModule { }
