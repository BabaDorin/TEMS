import { Property } from './view-property.model';
export interface IEquipmentType{
    id?: string;
    parents?: IEquipmentType[],
    name: string,
    properties: Property[],
    children: EquipmentType[];
}

export class EquipmentType implements IEquipmentType{
    id?: string;
    parents?: IEquipmentType[];
    name: string;
    properties: Property[];
    children: EquipmentType[];

    constructor(){
        this.parents = [];
        this.name = '';
        this.properties = [];
        this.children = [];
    }
}