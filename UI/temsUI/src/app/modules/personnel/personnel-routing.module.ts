import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CanViewEntitiesGuard } from 'src/app/guards/can-view-entities.guard';
import { PersonnelDetailsComponent } from 'src/app/tems-components/personnel/personnel-details/personnel-details.component';
import { ViewPersonnelComponent } from 'src/app/tems-components/personnel/view-personnel/view-personnel.component';
import { CanManageEntitiesGuard } from './../../guards/can-manage-entities.guard';
import { AddPersonnelComponent } from './../../tems-components/personnel/add-personnel/add-personnel.component';

const routes: Routes = [
  { path: '', component: ViewPersonnelComponent, canActivate: [CanViewEntitiesGuard] },
  { path: 'all', component: ViewPersonnelComponent, canActivate: [CanViewEntitiesGuard] },
  { path: 'add', component: AddPersonnelComponent, canActivate: [CanManageEntitiesGuard] },
  { path: 'details/:id', component: PersonnelDetailsComponent, canActivate: [CanViewEntitiesGuard] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PersonnelRoutingModule { }
