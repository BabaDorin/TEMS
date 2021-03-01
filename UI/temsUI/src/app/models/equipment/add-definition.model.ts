import { Type } from '@angular/core';
import { IOption } from '../option.model';
import { AddProperty } from './add-property.model';
import { AddType } from './add-type.model';

export interface IAddDefinition{
    id: string,
    identifier: string,
    equipmentType: IOption,
    properties: AddProperty[],
    children: AddDefinition[]
}

export class AddDefinition{
    id: string;
    identifier: string;
    equipmentType: IOption;
    properties: AddProperty[];
    children: AddDefinition[];

    constructor(){
        this.id = "";
        this.identifier = "";
        // this.equipmentType = new AddType();
        this.properties = [] as AddProperty[];
        this.children = [] as AddDefinition[];
    }
}




