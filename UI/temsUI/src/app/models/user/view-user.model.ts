export interface IViewUserSimplified{
    id: string;
    name: string;
}

export class ViewUserSimplified implements IViewUserSimplified{
    id: string;
    name: string;

    constructor(){
        this.id = '1';
        this.name = '2';
    }
}