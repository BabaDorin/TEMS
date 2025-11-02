import { IOption } from './../option.model';
import { DataType } from '../datatype.model';
export interface IAddProperty{
    id?: string,
    name: string,
    required?: boolean;
    displayName: string,
    description?: string,
    dataType: IOption,
    
    value?: string
    
    // number-specific
    min?: number;
    max?: number;

    // Checkbox, radiobutton & select specific
    options?: IOption[];
}

export class AddProperty implements IAddProperty{
    id?: string;
    name: string;
    required?: boolean;
    displayName: string;
    description?: string;
    dataType: IOption;
    value?: string;
    min?: number;
    max?: number;
    options?: IOption[];
}