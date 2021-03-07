export interface IViewEquipmentSimplified{
    id: string,
    temsId: string,
    serialNumber: string,
    temsidOrSn: string;
    definition: string,
    type: string,
    isUsed: boolean,
    isDefect: boolean;
    room: string;
}

export class ViewEquipmentSimplified implements IViewEquipmentSimplified{
    id: string;
    temsId: string;
    serialNumber: string;
    temsidOrSn: string;
    definition: string;
    type: string;
    isUsed: boolean;
    isDefect: boolean;
    room: string;

    constructor(){
        this.temsidOrSn = (this.temsId == undefined) ? this.temsId : this.serialNumber;
    }
}