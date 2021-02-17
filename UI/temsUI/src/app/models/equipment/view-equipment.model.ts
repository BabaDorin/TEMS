import { viewClassName } from '@angular/compiler';
import { ViewSimplifiedRoom } from './../room/view-room-simplified.model';
import { Property } from './view-property.model';
import { Type } from './view-type.model';

export interface IViewEquipment{
    id: string,
    identifier: string;
    temsID: string,
    serialNumber: string,
    room: ViewSimplifiedRoom;
    type: Type;
    specificTypeProperties: Property[],
    children: IViewEquipment[];
    parents: IViewEquipment[];
    idUsed: boolean,
    isDefect: boolean;
    price: number;
    photos: string[];
}

export class ViewEquipment implements IViewEquipment{
    id: string;
    identifier: string;
    temsID: string;
    serialNumber: string;
    room: ViewSimplifiedRoom;
    type: Type;
    children: IViewEquipment[];
    parents: IViewEquipment[];
    idUsed: boolean;
    isDefect: boolean;
    price: number;
    specificTypeProperties: Property[];
    photos: string[];

    constructor(){
        this.id = "equipmentID";
        this.identifier = "equipmentIdentifier";
        this.serialNumber = "equipmentSerialNumber";
        this.room = { id: '1', identifier: '307'};
        this.idUsed = true;
        this.isDefect = false;
        this.children = [];
        this.parents = [];
        this.specificTypeProperties = [
            { id: '1', dataType: { id: '1', name: 'string' }, displayName: 'Model', description: 'Items model' },
            { id: '2', dataType: { id: '2', name: 'number' }, displayName: 'Frecventa', description: 'F in Hz' },
        ];
 
        this.photos = ['img1', 'img2'];
    }
}