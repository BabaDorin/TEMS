import { CanManageEntitiesGuard } from './../../guards/can-manage-entities.guard';
import { CanViewEntitiesGuard } from './../../guards/can-view-entities.guard';
import { CreateIssueComponent } from 'src/app/tems-components/issue/create-issue/create-issue.component';
import { ViewIssuesComponent } from './../../tems-components/issue/view-issues/view-issues.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', component: ViewIssuesComponent, canActivate: [CanViewEntitiesGuard]},
  { path: 'all', component: ViewIssuesComponent, canActivate: [CanViewEntitiesGuard] },
  { path: 'create', component: CreateIssueComponent, },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class IssuesRoutingModule { }
