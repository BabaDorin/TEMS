import { ViewEquipmentAllocation } from './../../models/equipment/view-equipment-allocation.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AllocationService {

  getEquipmentAllocations(equipmentId: string){
    return [
      new ViewEquipmentAllocation(),
      new ViewEquipmentAllocation(),
      new ViewEquipmentAllocation(),
      new ViewEquipmentAllocation(),
      new ViewEquipmentAllocation(),
    ];
  }

  getEquipmentAllocationsToRoom(roomId: string){
    return this.getEquipmentAllocations('1'); // testing purposes!
  }

  getEquipmentAllocationsToPersonnel(personnelId: string){
    return this.getEquipmentAllocations('1'); // testing purposes!
  }
}
