export interface IAddRoom{
    identifier: string,
    floor: number,
    description: string,
    label: string,
}

export class AddRoom implements IAddRoom{
    identifier: string;
    floor: number;
    description: string;
    label: string;

    constructor(){
        this.identifier = ""
        this.floor = 1;
        this.description = "";
        this.label = "";
    }
}