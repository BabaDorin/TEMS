import { IOption } from 'src/app/models/option.model';

export interface IProperty{
    id?: string, // we do not need the ID if we use it only to display property value
    displayName: string,
    name?: string,
    description?: string,
    dataType?: string, // undefined = string
    value?: any
    required?: boolean,
    min?: number,
    max?: number,
    options?: IOption[],
}

export class Property implements IProperty{
    id?: string;
    name?: string;
    displayName: string;
    description?: string;
    dataType?: string;
    value?: any;
    required?: boolean;
    min?: number;
    max?: number;
    options?: IOption[];
}