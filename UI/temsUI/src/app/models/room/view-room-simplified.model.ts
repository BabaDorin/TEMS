export interface IViewRoomSimplified{
    id: string,
    identifier: string,
}

export class ViewRoomSimplified implements IViewRoomSimplified{
    id: string;
    identifier: string;
    
    constructor(){
        this.id = '1';
        this.identifier = '307';
    }
}