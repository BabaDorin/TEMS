import { DataType } from '../datatype.model';
export interface Property{
    id: string,
    displayName: string,
    description: string,
    dataType: DataType
}