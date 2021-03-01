export interface IViewKeySimplified{
    id: string;
    identifier: string;
    numberOfCopies: number;
    isAllocated: boolean;
    description?: string;
}

export class ViewKeySimplified implements IViewKeySimplified{
    id: string;
    identifier: string;
    numberOfCopies: number;
    isAllocated: boolean;
    description?: string;

    constructor(){
        this.id = '1';
        this.identifier = '307';
        this.numberOfCopies = 3;
        this.isAllocated = true;
        this.description = 'cheia de la deposit';
    }
}