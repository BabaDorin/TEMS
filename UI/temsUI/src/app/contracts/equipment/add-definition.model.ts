import { AddProperty } from './add-property.model';
import { AddType } from './add-type.model';
export interface AddDefinition{
    id: string,
    identifier: string,
    equipmentType: AddType,
    properties: AddProperty[],
    
    children: AddDefinition[]
}