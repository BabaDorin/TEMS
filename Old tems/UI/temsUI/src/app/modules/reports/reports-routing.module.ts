import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CanViewEntitiesGuard } from 'src/app/guards/can-view-entities.guard';
import { ReportsComponent } from 'src/app/tems-components/reports/reports.component';
import { CreateReportTemplateComponent } from '../../tems-components/reports/create-report-template/create-report-template.component';
import { CanManageEntitiesGuard } from './../../guards/can-manage-entities.guard';

const routes: Routes = [
  { path: '', component: ReportsComponent, canActivate: [CanViewEntitiesGuard] },
  { path: 'createtemplate', component: CreateReportTemplateComponent, canActivate: [CanManageEntitiesGuard] },
  { path: 'updatetemplate/:id', component: CreateReportTemplateComponent, canActivate: [CanManageEntitiesGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportsRoutingModule { }
