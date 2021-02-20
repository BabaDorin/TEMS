export interface IViewPersonnelSimplified{
    id: string;
    name: string;
}

export class ViewPersonnelSimplified implements IViewPersonnelSimplified{
    id: string;
    name: string;
    
    constructor(){
        this.id = '1';
        this.name = 'Baba Dory';
    }
}