import { AllocateEquipmentGuard } from './../../guards/equipment-guards/allocate-equipment/allocate-equipment.guard';
import { EquipmentAllocationComponent } from './equipment-allocation/equipment-allocation.component';
import { AddEquipmentComponent } from './add-equipment/add-equipment.component';
import { ViewEquipmentComponent } from './view-equipment/view-equipment.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EquipmentDetailsComponent } from './equipment-details/equipment-details.component';
import { QuickAccessComponent } from './quick-access/quick-access.component';
import { ViewEquipmentGuard } from 'src/app/guards/equipment-guards/view-equipment/view-equipment.guard';

const routes: Routes = [
  { path: '', component: ViewEquipmentComponent, canActivate: [ ViewEquipmentGuard ] },
  { path: 'add', component: AddEquipmentComponent, canActivate: [ ViewEquipmentGuard ] },
  { path: 'details/:id', component: EquipmentDetailsComponent, canActivate: [ ViewEquipmentGuard ] },
  { path: 'all', component: ViewEquipmentComponent, canActivate: [ ViewEquipmentGuard ] },
  { path: 'quick-access', component: QuickAccessComponent, canActivate: [ ViewEquipmentGuard ] },
  { path: 'allocate', component: EquipmentAllocationComponent, canActivate: [ AllocateEquipmentGuard ] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EquipmentRoutingModule { }
