import { ViewRoomSimplified } from '../room/view-room-simplified.model';
import { ViewPersonnelSimplified } from './../personnel/view-personnel-simplified.model';
import { IViewEquipmentSimplified, ViewEquipmentSimplified } from './view-equipment-simplified.model';

export interface IViewEquipmentAllocation{
    id: string,
    equipment: ViewEquipmentSimplified,
    room?: ViewRoomSimplified;
    personnel?: ViewPersonnelSimplified
    dateAllocated: Date;
    dateReturned?: Date;
}

export class ViewEquipmentAllocation implements IViewEquipmentAllocation{
    id: string;
    equipment: ViewEquipmentSimplified;
    room?: ViewRoomSimplified;
    personnel?: ViewPersonnelSimplified;
    dateAllocated: Date;
    dateReturned?: Date;

    constructor(){
        this.id = '1';
        this.equipment = new ViewEquipmentSimplified();
        this.room = new ViewRoomSimplified();
        this.personnel = undefined;
        this.dateAllocated = new Date();
        this.dateReturned = new Date();
    }
}
