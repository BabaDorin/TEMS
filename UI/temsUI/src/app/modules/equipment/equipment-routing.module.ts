import { TemsidGeneratorComponent } from './../../tems-components/temsid-generator/temsid-generator.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AddEquipmentComponent } from 'src/app/tems-components/equipment/add-equipment/add-equipment.component';
import { EquipmentAllocationComponent } from 'src/app/tems-components/equipment/equipment-allocation/equipment-allocation.component';
import { EquipmentDetailsComponent } from 'src/app/tems-components/equipment/equipment-details/equipment-details.component';
import { QuickAccessComponent } from 'src/app/tems-components/equipment/quick-access/quick-access.component';
import { ViewEquipmentComponent } from 'src/app/tems-components/equipment/view-equipment/view-equipment.component';
import { ViewEquipmentAllocationsComponent } from 'src/app/tems-components/view-equipment-allocations/view-equipment-allocations.component';
import { CanManageEntitiesGuard } from './../../guards/can-manage-entities.guard';
import { CanViewEntitiesGuard } from './../../guards/can-view-entities.guard';

const routes: Routes = [
  { path: '', component: ViewEquipmentComponent, canActivate: [ CanViewEntitiesGuard ] },
  { path: 'add', component: AddEquipmentComponent, canActivate: [ CanManageEntitiesGuard ] },
  { path: 'details/:id', component: EquipmentDetailsComponent, canActivate: [ CanViewEntitiesGuard ] },
  { path: 'all', component: ViewEquipmentComponent, canActivate: [ CanViewEntitiesGuard ] },
  { path: 'quick-access', component: QuickAccessComponent, canActivate: [ CanViewEntitiesGuard ] },
  { path: 'allocate', component: EquipmentAllocationComponent, canActivate: [ CanManageEntitiesGuard ] },
  { path: 'allocations', component: ViewEquipmentAllocationsComponent, canActivate: [ CanViewEntitiesGuard ] },
  { path: 'generate-temsid', component: TemsidGeneratorComponent, canActivate: [ CanViewEntitiesGuard ] },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EquipmentRoutingModule { }
