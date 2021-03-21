import { CanManageEntitiesGuard } from './../../guards/can-manage-entities.guard';
import { CreateReportTemplateComponent } from './../../tems-components/reports/create-report-template/create-report-template.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReportsComponent } from 'src/app/tems-components/reports/reports.component';
import { CanViewEntitiesGuard } from 'src/app/guards/can-view-entities.guard';

const routes: Routes = [
  { path: '', component: ReportsComponent, canActivate: [CanViewEntitiesGuard] },
  { path: 'createTemplate', component: CreateReportTemplateComponent, canActivate: [CanManageEntitiesGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportsRoutingModule { }
