import { DataType } from '../datatype.model';
export interface Property{
    id?: string, // we do not need the ID if we use it only to display property value
    displayName: string,
    description?: string,
    dataType?: DataType, // undefined = string
    value: any
}