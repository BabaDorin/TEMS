import { AddProperty } from './add-property.model';
 export interface IAddType{
    parents?: IAddType[],
    id: string,
    name: string,
    properties: AddProperty[];
}

export class AddType implements IAddType{
    parents?: IAddType[];
    children?: IAddType[];

    id: string;
    name: string;
    properties: AddProperty[];

    constructor(){
        this.parents = [];
        this.name = '';
        this.id = '';
        this.properties = [];
    }
}