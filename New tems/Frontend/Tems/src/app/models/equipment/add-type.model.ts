import { IOption } from 'src/app/models/option.model';

export interface IAddType{
    id?: string,
    parents?: IOption[],
    name: string,
    properties: IOption[];
}

export class AddType implements IAddType{
    id?: string;
    parents?: IOption[];
    name: string;
    properties: IOption[];
}