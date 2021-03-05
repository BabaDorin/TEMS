import { EquipmentType } from './view-type.model';
import { IOption } from '../option.model';
import { AddProperty } from './add-property.model';
import { Property } from './view-property.model';

export interface IDefinition{
    type: EquipmentType;
    id: string,
    identifier: string,
    equipmentType: IOption,
    properties: Property[],
    children: Definition[],
    price?: number,
    currency?: string
}

export class Definition implements IDefinition{
    type: EquipmentType;
    id: string;
    identifier: string;
    equipmentType: IOption;
    properties: Property[];
    children: Definition[];
    price?: number;
    currency?: string

    constructor(){
        this.type = new EquipmentType()
        this.id = "";
        this.identifier = "";
        // this.equipmentType = new AddType();
        this.properties = [] as Property[];
        this.children = [] as Definition[];
    }
}




