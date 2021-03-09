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
    closedBy?: ViewUserSimplified,
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
    closedBy?: ViewUserSimplified;
    dateCreated: Date;
    dateClosed?: Date;

    constructor(){
        this.id = '1';
        this.problem = 'Este nevoie de interventia unui tehnician';
        this.problemDescription = 'Nu exista access la internet in cabinetul 204';
        let room = new ViewRoomSimplified();
        room.identifier = '204';
        this.rooms = [ room ];
        let personnel = new ViewPersonnelSimplified();
        personnel.name = 'Arnold Schartzneger';
        this.personnel =  [ personnel ];
        this.status = 'urgent';
        this.dateCreated = new Date();
    }
}