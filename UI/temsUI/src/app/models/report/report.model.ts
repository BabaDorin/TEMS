import { IViewUserSimplified } from './../user/view-user.model';
import { IOption } from 'src/app/models/option.model';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
export interface IReport{
    id?: string,
    name: string,
    description?: string,
    isDefault?: boolean;

    rooms?: IOption[],
    personnel?: IOption[],
    equipment?: IOption[],
    types?: IOption[],

    fields?: IReportField[],

    createdBy?: IViewUserSimplified,

    exportTo: string, // 'word', 'excel'

    header?: string;
    footer?: string;

    signatories?: IOption[]; 
}

interface IReportField{
    identifier: string, // 'price'
    filter?: string[], // ['> 5', '< 10'] etc.
    sort?: string, // 'asc' / 'desc' / udefined
}

export class Report implements IReport
{
    id: string;
    name: string;
    description: string;
    isDefault?: boolean = true;
    rooms: IOption[];
    personnel: IOption[];
    equipment: IOption[];
    types: IOption[];
    fields: IReportField[];
    createdBy: IViewUserSimplified;
    exportTo: string;
    header: string;
    footer: string;
    signatories: IOption[];

    constructor(){
        this.id = '1';
        this.name = 'custom report';
        this.description = 'custom report description';
        this.rooms = [ { id: '1', value: '304'}];
        this.personnel = [ { id: '1', value: 'Baba'}];
        this.equipment = [ { id: '1', value: 'serialNumber'}];
        this.exportTo = 'excel';
    }
}