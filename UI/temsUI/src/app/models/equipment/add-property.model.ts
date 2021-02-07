import { DataType } from '../datatype.model';
export interface AddProperty{
    id: string,
    name: string,
    displayName: string,
    description: string,
    dataType: DataType,
    value?: string
}