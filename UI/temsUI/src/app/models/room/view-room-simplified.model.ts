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
}