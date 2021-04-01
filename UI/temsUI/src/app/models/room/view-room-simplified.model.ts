export interface IViewRoomSimplified{
    id: string;
    identifier: string;
    activeIssues?: number;
    description?: string;
    label?: string;
    allocatedEquipment?: number;
    isArchieved: boolean,
}

export class ViewRoomSimplified implements IViewRoomSimplified{
    id: string;
    identifier: string;
    activeIssues?: number;
    description?: string;
    label?: string;
    allocatedEquipment?: number;
    isArchieved: boolean;
}