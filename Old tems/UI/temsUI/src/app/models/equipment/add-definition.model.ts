import { IOption } from 'src/app/models/option.model';
import { EquipmentType } from './view-type.model';
import { AddProperty } from './add-property.model';
import { Property } from './view-property.model';

export class AddDefinition {
    id: string;
    typeId: string;
    identifier: string;
    description?: string;
    price: number;
    currency: string;
    properties = [] as IOption[];
    children = [] as AddDefinition[];
}

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
    currency?: string;

    constructor(){
        this.properties = [] as Property[];
        this.children = [] as Definition[];
    }
}



