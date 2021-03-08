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
    identifier: string;
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
    identifier: string;
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

    // constructor(){
    //     this.id = "equipmentID";
    //     this.identifier = "Asus Vivobook";
    //     this.temsId = "LPB001";
    //     this.serialNumber = "3044245435";
    //     this.type = new EquipmentType();
    //     this.room = { id: '1', identifier: '307'};
    //     this.isUsed = true;
    //     this.isDefect = true;
    //     this.children = [
    //         {id: '1', type: 'Processor', identifier: 'Intel Core i5 1050'},
    //         {id: '2', type: 'Graphics Card', identifier: 'Nvidia Geforce 1650'},
    //     ];
    //     this.parents = [];
    //     this.specificTypeProperties = [
    //         { id: '1', dataType: { id: '1', name: 'string' }, displayName: 'Model', description: 'Items model', value: "cf" },
    //         { id: '2', dataType: { id: '2', name: 'number' }, displayName: 'Frecventa', description: 'F in Hz', value: "4200"  },
    //     ];
 
    //     this.photos = ['img1', 'img2'];
    //     this.price = 0;
    // }
}