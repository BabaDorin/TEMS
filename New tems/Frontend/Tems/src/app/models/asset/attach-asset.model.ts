export interface IAttachEquipment{
    parentId: string,
    childrenIds: string[],
}

export class AttachEquipment implements IAttachEquipment{
    parentId: string;
    childrenIds: string[];
}