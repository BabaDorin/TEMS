export interface IViewEquipmentSimplified{
    id: string,
    temsID: string,
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
    temsID: string;
    serialNumber: string;
    temsidOrSn: string;
    definition: string;
    type: string;
    isUsed: boolean;
    isDefect: boolean;
    room: string;

    constructor(){
        this.id = "id";
        this.temsID = "temsID";
        this.serialNumber = "serialNumber";
        this.temsidOrSn = (this.temsID == undefined) ? this.temsID : this.serialNumber;
        this.definition = "definition";
        this.type = "type";
        this.isDefect = false;
        this.isUsed = true;
        this.room = "307";
    }
}