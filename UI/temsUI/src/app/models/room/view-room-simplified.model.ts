export interface IViewRoomSimplified{
    id: string;
    identifier: string;
    activeIssues?: number;
    description?: string;
    label?: string;
    allocatedEquipment?: number;
}

export class ViewRoomSimplified implements IViewRoomSimplified{
    id: string;
    identifier: string;
    activeIssues?: number;
    description?: string;
    label?: string;
    allocatedEquipment?: number;
    
    constructor(){
        this.id = '1';
        this.identifier = '307';
        this.activeIssues = 1;
        this.description = 'Depozit =)'
        this.label = 'depozit';
        this.allocatedEquipment = 13;
    }
}