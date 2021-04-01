import { IOption } from './../option.model';
interface IViewPersonnel {
    id: string,
    name: string,
    phoneNumber?: string,
    email?: string,
    positions?: IOption,
    roomSupervisories?: IOption[],
    activeTickets?: number,
    allocatedEquipments?: number,
    isArchieved: boolean,
    // imagePath: string -- TO BE IMPLEMENTED LATER
}

export class ViewPersonnel implements IViewPersonnel{
    id: string;
    name: string;
    phoneNumber?: string;
    email?: string;
    positions?: IOption;
    roomSupervisories?: IOption[];
    activeTickets?: number;
    allocatedEquipments?: number;
    isArchieved: boolean;
}