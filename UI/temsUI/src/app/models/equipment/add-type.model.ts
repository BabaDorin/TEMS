import { IOption } from 'src/app/models/option.model';

export interface IAddType{
    parents?: IOption[],
    id?: string,
    name: string,
    properties: IOption[];
}

export class AddType implements IAddType{
    parents?: IOption[];

    id?: string;
    name: string;
    properties: IOption[];

    constructor(){
        this.parents = [];
        this.name = '';
        this.properties = [];
    }
}