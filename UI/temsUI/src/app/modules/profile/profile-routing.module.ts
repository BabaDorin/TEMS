import { ViewProfileComponent } from './../../tems-components/view-profile/view-profile.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: 'view', component: ViewProfileComponent },
  { path: 'view/:id', component: ViewProfileComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProfileRoutingModule { }
