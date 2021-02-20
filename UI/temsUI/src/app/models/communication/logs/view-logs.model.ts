import { ViewSimplifiedRoom } from '../../room/view-room-simplified.model';
import { ViewEquipmentSimplified } from './../../equipment/view-equipment-simplified.model';
import { LogType } from './log-type.model';

export interface IViewLog {
    id: string;
    dateCreated: Date;
    text?: string;
    equipment?: ViewEquipmentSimplified;
    room?: ViewSimplifiedRoom;
    personnel?: ViewSimplifiedRoom;
    logType: LogType;
    isImportant: boolean;
}

export class ViewLog implements IViewLog {
    id: string;
    dateCreated: Date;
    text?: string;
    equipment?: ViewEquipmentSimplified;
    room?: ViewSimplifiedRoom;
    personnel?: ViewSimplifiedRoom;
    logType: LogType;
    isImportant: boolean;

    constructor() {
        this.id = '1';
        this.dateCreated = new Date();
        this.text = 'Equipment has been repaired';
        this.equipment = {
            id: '1',
            temsID: 'LPB001',
            serialNumber: '22344',
            definition: 'Asus VivoBook',
            type: 'Laptoc',
            isUsed: true,
            isDefect: true,
            room: '307'
        };
        this.logType = { id: '1', type: 'Simple Log Type' };
        this.isImportant = true;
    }
}





