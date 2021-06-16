import { ViewEquipmentAllocationsComponent } from './../view-equipment-allocations/view-equipment-allocations.component';
import { CanManageEntitiesGuard } from './../../guards/can-manage-entities.guard';
import { CanViewEntitiesGuard } from './../../guards/can-view-entities.guard';
import { EquipmentAllocationComponent } from './equipment-allocation/equipment-allocation.component';
import { AddEquipmentComponent } from './add-equipment/add-equipment.component';
import { ViewEquipmentComponent } from './view-equipment/view-equipment.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EquipmentDetailsComponent } from './equipment-details/equipment-details.component';
import { QuickAccessComponent } from './quick-access/quick-access.component';

const routes: Routes = [
  { path: '', component: ViewEquipmentComponent, canActivate: [ CanViewEntitiesGuard ] },
  { path: 'add', component: AddEquipmentComponent, canActivate: [ CanManageEntitiesGuard ] },
  { path: 'details/:id', component: EquipmentDetailsComponent, canActivate: [ CanViewEntitiesGuard ] },
  { path: 'all', component: ViewEquipmentComponent, canActivate: [ CanViewEntitiesGuard ] },
  { path: 'quick-access', component: QuickAccessComponent, canActivate: [ CanViewEntitiesGuard ] },
  { path: 'allocate', component: EquipmentAllocationComponent, canActivate: [ CanManageEntitiesGuard ] },
  { path: 'allocations', component: ViewEquipmentAllocationsComponent, canActivate: [ CanViewEntitiesGuard ] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EquipmentRoutingModule { }
