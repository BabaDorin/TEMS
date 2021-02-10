 export interface IAddType{
    parents?: IAddType[],
    id: string,
    name: string,
}

export class AddType implements IAddType{
    parents?: IAddType[];
    id: string;
    name: string;

    constructor(){
        this.parents = [];
        this.name = '';
        this.id = '';
    }
}