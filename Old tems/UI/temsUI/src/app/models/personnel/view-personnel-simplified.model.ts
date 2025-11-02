import { IOption } from './../option.model';
export interface IViewPersonnelSimplified{
    id: string,
    name: string,
    allocatedEquipments?: number,
    activeTickets?:number,
    positions?: string,
    isArchieved: boolean,
}

export class ViewPersonnelSimplified implements IViewPersonnelSimplified{
    id: string;
    name: string;
    allocatedEquipment?: number;
    activeTickets?:number;
    pozitions?: string;
    isArchieved: boolean;
}