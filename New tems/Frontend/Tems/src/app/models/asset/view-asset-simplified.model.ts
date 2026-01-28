export interface IViewAssetSimplified{
    id: string,
    temsId: string,
    serialNumber: string,
    temsIdOrSerialNumber: string;
    definition: string,
    type: string,
    isUsed: boolean,
    isDefect: boolean;
    isArchieved: boolean;
    assignee: string;
}

export class ViewAssetSimplified implements IViewAssetSimplified{
    id: string;
    temsId: string;
    serialNumber: string;
    temsIdOrSerialNumber: string;
    definition: string;
    type: string;
    isUsed: boolean;
    isDefect: boolean;
    isArchieved: boolean;
    assignee: string;

    constructor(){
        // this.temsidOrSn = (this.temsId == undefined) ? this.temsId : this.serialNumber;
    }
}