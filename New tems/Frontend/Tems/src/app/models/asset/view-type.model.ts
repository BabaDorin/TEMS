import { IOption } from './../option.model';
import { Property } from './view-property.model';
export interface IAssetType{
    id?: string;
    parents: IOption[],
    name: string,
    editable: boolean,
    properties: Property[],
    children: AssetType[];
}

export class AssetType implements IAssetType{
    id?: string;
    parents: IOption[];
    name: string;
    editable: boolean;
    properties: Property[];
    children: AssetType[];
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