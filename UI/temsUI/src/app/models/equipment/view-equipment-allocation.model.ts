import { ViewRoomSimplified } from '../room/view-room-simplified.model';
import { ViewPersonnelSimplified } from './../personnel/view-personnel-simplified.model';
import { ViewEquipmentSimplified } from './view-equipment-simplified.model';

export interface IViewAllocationSimplified{
    id: string,
    equipment: ViewEquipmentSimplified,
    room?: ViewRoomSimplified;
    personnel?: ViewPersonnelSimplified
    dateAllocated: Date;
    dateReturned?: Date;
}

export class ViewAllocationSimplified implements IViewAllocationSimplified{
    id: string;
    equipment: ViewEquipmentSimplified;
    room?: ViewRoomSimplified;
    personnel?: ViewPersonnelSimplified;
    dateAllocated: Date;
    dateReturned?: Date;
}
