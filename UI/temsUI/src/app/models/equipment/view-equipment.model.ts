import { IOption } from 'src/app/models/option.model';
import { viewClassName } from '@angular/compiler';
import { ViewRoomSimplified } from './../room/view-room-simplified.model';
import { Property } from './view-property.model';
import { EquipmentType } from './view-type.model';

export interface ILightViewEquipment{
    id: string;
    type: string;
    identifier: string;
}

export interface IViewEquipment{
    id: string,
    definition: IOption;
    temsId: string,
    serialNumber: string,
    room: IOption;
    type: EquipmentType;
    specificTypeProperties: Property[],
    children: IOption[];
    parent: IOption;
    isUsed: boolean,
    isDefect: boolean;
    price: number;
    // photos: string[];
}

export class ViewEquipment implements IViewEquipment{
    id: string;
    definition: IOption;
    temsId: string;
    serialNumber: string;
    room: IOption;
    type: EquipmentType;
    children: IOption[];
    parent: IOption;
    isUsed: boolean;
    isDefect: boolean;
    price: number;
    specificTypeProperties: Property[];
    photos: string[];
}