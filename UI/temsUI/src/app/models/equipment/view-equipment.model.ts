import { viewClassName } from '@angular/compiler';
import { ViewRoomSimplified } from './../room/view-room-simplified.model';
import { Property } from './view-property.model';
import { Type } from './view-type.model';

export interface ILightViewEquipment{
    id: string;
    type: string;
    identifier: string;
}

export interface IViewEquipment{
    id: string,
    identifier: string;
    temsID: string,
    serialNumber: string,
    room: ViewRoomSimplified;
    type: Type;
    specificTypeProperties: Property[],
    children: ILightViewEquipment[];
    parents: ILightViewEquipment[];
    isUsed: boolean,
    isDefect: boolean;
    price: number;
    photos: string[];
}

export class ViewEquipment implements IViewEquipment{
    id: string;
    identifier: string;
    temsID: string;
    serialNumber: string;
    room: ViewRoomSimplified;
    type: Type;
    children: ILightViewEquipment[];
    parents: ILightViewEquipment[];
    isUsed: boolean;
    isDefect: boolean;
    price: number;
    specificTypeProperties: Property[];
    photos: string[];

    constructor(){
        this.id = "equipmentID";
        this.identifier = "Asus Vivobook";
        this.temsID = "LPB001";
        this.serialNumber = "3044245435";
        this.type = { id: '1', name: 'Laptop' }
        this.room = { id: '1', identifier: '307'};
        this.isUsed = true;
        this.isDefect = true;
        this.children = [
            {id: '1', type: 'Processor', identifier: 'Intel Core i5 1050'},
            {id: '2', type: 'Graphics Card', identifier: 'Nvidia Geforce 1650'},
        ];
        this.parents = [];
        this.specificTypeProperties = [
            { id: '1', dataType: { id: '1', name: 'string' }, displayName: 'Model', description: 'Items model', value: "cf" },
            { id: '2', dataType: { id: '2', name: 'number' }, displayName: 'Frecventa', description: 'F in Hz', value: "4200"  },
        ];
 
        this.photos = ['img1', 'img2'];
        this.price = 0;
    }
}