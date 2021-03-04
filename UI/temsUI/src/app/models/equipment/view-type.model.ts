import { Property } from './view-property.model';
export interface IEquipmentType{
    parents?: IEquipmentType[],
    name: string,
    properties: Property[],
    children: IEquipmentType[];
}

export class EquipmentType implements IEquipmentType{
    parents?: IEquipmentType[];
    name: string;
    properties: Property[];
    children: IEquipmentType[];

    constructor(){
        this.parents = [];
        this.name = '';
        this.properties = [];
        this.children = [];
    }
}