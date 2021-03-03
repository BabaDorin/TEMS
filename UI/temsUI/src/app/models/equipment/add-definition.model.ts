import { Type } from './view-type.model';
import { IOption } from '../option.model';
import { AddProperty } from './add-property.model';

export interface IDefinition{
    type: Type;
    id: string,
    identifier: string,
    equipmentType: IOption,
    properties: AddProperty[],
    children: Definition[],
    price?: number,
    currency?: string
}

export class Definition implements IDefinition{
    type: Type;
    id: string;
    identifier: string;
    equipmentType: IOption;
    properties: AddProperty[];
    children: Definition[];
    price?: number;
    currency?: string

    constructor(){
        this.type = new Type()
        this.id = "";
        this.identifier = "";
        // this.equipmentType = new AddType();
        this.properties = [] as AddProperty[];
        this.children = [] as Definition[];
    }
}




