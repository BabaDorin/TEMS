import { Definition } from './add-definition.model';
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
    definition: Definition;
    temsId: string,
    serialNumber: string,
    room: IOption,
    personnel: IOption,
    type: EquipmentType,
    specificProperties: Property[],
    children: IOption[],
    parent: IOption;
    isUsed: boolean,
    isDefect: boolean,
    price: number,
    isArchieved: boolean,
}

export class ViewEquipment implements IViewEquipment{
    id: string;
    definition: Definition;
    temsId: string;
    serialNumber: string;
    room: IOption;
    personnel: IOption;
    type: EquipmentType;
    children: IOption[];
    parent: IOption;
    isUsed: boolean;
    isDefect: boolean;
    price: number;
    specificProperties: Property[];
    isArchieved: boolean;
}