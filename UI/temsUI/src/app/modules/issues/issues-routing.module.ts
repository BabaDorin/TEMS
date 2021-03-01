import { CreateIssueComponent } from 'src/app/tems-components/issue/create-issue/create-issue.component';
import { ViewIssuesComponent } from './../../tems-components/issue/view-issues/view-issues.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', component: ViewIssuesComponent },
  { path: 'all', component: ViewIssuesComponent },
  { path: 'create', component: CreateIssueComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class IssuesRoutingModule { }
