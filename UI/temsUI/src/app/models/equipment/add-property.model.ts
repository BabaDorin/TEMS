import { DataType } from '../datatype.model';
export interface AddProperty{
    id?: string,
    name: string,
    required?: boolean;
    displayName: string,
    description?: string,
    dataType: DataType,
    
    value?: string
    
    // number-specific
    min?: number;
    max?: number;

    // Checkbox, radiobutton & select specific
    options?: PropertyOption[];
}

export interface PropertyOption{
    value,
    label
}