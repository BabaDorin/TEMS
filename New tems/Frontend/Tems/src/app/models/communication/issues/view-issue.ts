import { IOption } from 'src/app/models/option.model';
import { ViewUserSimplified } from './../../user/view-user.model';
import { ViewPersonnelSimplified } from './../../personnel/view-personnel-simplified.model';
import { ViewEquipmentSimplified } from './../../equipment/view-equipment-simplified.model';
import { ViewRoomSimplified } from '../../room/view-room-simplified.model';

export interface IViewIssue {
    id: string,
    problem: string;
    problemDescription?: string,
    roomIdentifier?: ViewRoomSimplified[],    
    equipmentIdentifier?: ViewEquipmentSimplified[],    
    personnelIdentifier?: ViewPersonnelSimplified[],
    status: string,
    asignees?: ViewUserSimplified[],
    createdBy?: ViewUserSimplified,
    closedBy?: IOption,
    inProgress: boolean;
    dateCreated: Date,
    dateClosed?: Date,
}

export class ViewIssue implements IViewIssue{
    id: string;
    problem: string;
    problemDescription?: string;
    rooms?: ViewRoomSimplified[];
    equipment?: ViewEquipmentSimplified[];
    personnel?: ViewPersonnelSimplified[];
    status: string;
    asignees?: ViewUserSimplified[];
    createdBy?: ViewUserSimplified;
    closedBy?: IOption;
    dateCreated: Date;
    inProgress: boolean = false;
    dateClosed?: Date;
}