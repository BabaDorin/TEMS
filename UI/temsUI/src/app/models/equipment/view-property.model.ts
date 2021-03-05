import { IOption } from 'src/app/models/option.model';
import { DataType } from '../datatype.model';
export interface IProperty{
    id?: string, // we do not need the ID if we use it only to display property value
    displayName: string,
    name?: string,
    description?: string,
    dataType?: DataType, // undefined = string
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
    dataType?: DataType;
    value?: any;
    required?: boolean;
    min?: number;
    max?: number;
    options?: IOption[];
}