import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CanViewEntitiesGuard } from 'src/app/guards/can-view-entities.guard';
import { ViewArchieveComponent } from './../../tems-components/archieve/view-archieve/view-archieve.component';

const routes: Routes = [
  { path: '', component: ViewArchieveComponent, canActivate: [ CanViewEntitiesGuard ] },
  { path: '/:id', component: ViewArchieveComponent, canActivate: [ CanViewEntitiesGuard ] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ArchieveRoutingModule { }
