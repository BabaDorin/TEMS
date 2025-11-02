import { IOption } from './../option.model';
import { Property } from './view-property.model';
export interface IEquipmentType{
    id?: string;
    parents: IOption[],
    name: string,
    editable: boolean,
    properties: Property[],
    children: EquipmentType[];
}

export class EquipmentType implements IEquipmentType{
    id?: string;
    parents: IOption[];
    name: string;
    editable: boolean;
    properties: Property[];
    children: EquipmentType[];
}

export interface IViewType{
    id: string,
    name: string,
    editable: boolean,
    parents: IOption[],
    children: ViewType[],
    properties: IOption[]
}

export class ViewType implements IViewType{
    id: string;
    name: string;
    editable: boolean;
    parents: IOption[];
    children: ViewType[];
    properties: IOption[];
}