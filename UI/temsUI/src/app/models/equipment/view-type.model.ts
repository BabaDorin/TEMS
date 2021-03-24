import { IOption } from './../option.model';
import { Property } from './view-property.model';
export interface IEquipmentType{
    id?: string;
    parent: IEquipmentType,
    name: string,
    properties: Property[],
    children: EquipmentType[];
}

export class EquipmentType implements IEquipmentType{
    id?: string;
    parent: IEquipmentType;
    name: string;
    properties: Property[];
    children: EquipmentType[];
}

export interface IViewType{
    id: string,
    name: string,
    parent: IOption,
    children: IOption[],
    properties: IOption[]
}

export class ViewType implements IViewType{
    id: string;
    name: string;
    parent: IOption;
    children: IOption[];
    properties: IOption[];
}