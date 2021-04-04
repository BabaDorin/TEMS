import { CanManageSystemConfigurationGuard } from './../../guards/can-manage-system-configuration.guard';
import { CanManageEntitiesGuard } from './../../guards/can-manage-entities.guard';
import { CanViewEntitiesGuard } from 'src/app/guards/can-view-entities.guard';
import { ViewProfileComponent } from './../../tems-components/view-profile/view-profile.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: 'view', component: ViewProfileComponent, canActivate: [CanViewEntitiesGuard]},
  { path: 'view/:id', component: ViewProfileComponent, canActivate: [CanViewEntitiesGuard] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProfileRoutingModule { }
