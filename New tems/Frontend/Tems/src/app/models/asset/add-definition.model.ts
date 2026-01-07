import { IOption } from 'src/app/models/option.model';
import { AssetType } from './view-type.model';
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
    type: AssetType;
    id: string,
    identifier: string,
    assetType: IOption,
    properties: Property[],
    children: Definition[],
    price?: number,
    currency?: string
}

export class Definition implements IDefinition{
    type: AssetType;
    id: string;
    identifier: string;
    assetType: IOption;
    properties: Property[];
    children: Definition[];
    price?: number;
    currency?: string;

    constructor(){
        this.properties = [] as Property[];
        this.children = [] as Definition[];
    }
}



