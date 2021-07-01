import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CreateIssueComponent } from 'src/app/tems-components/issue/create-issue/create-issue.component';
import { CanViewEntitiesGuard } from './../../guards/can-view-entities.guard';
import { ViewIssuesComponent } from './../../tems-components/issue/view-issues/view-issues.component';

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
