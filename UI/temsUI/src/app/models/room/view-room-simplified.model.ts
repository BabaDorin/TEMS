export interface IViewSimplifiedRoom{
    id: string,
    identifier: string,
}

export class ViewSimplifiedRoom implements IViewSimplifiedRoom{
    id: string;
    identifier: string;

    constructor(){
        this.id = "room ID";
        this.identifier = "room identifier";
    }
}