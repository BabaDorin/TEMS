import { IOption } from './../option.model';
export interface IViewKeySimplified{
    id: string;
    identifier: string;
    numberOfCopies: number;
    allocatedTo: IOption;
    timePassed: Date;
    description?: string;
}

export class ViewKeySimplified implements IViewKeySimplified{
    id: string;
    identifier: string;
    numberOfCopies: number;
    allocatedTo: IOption;
    timePassed: Date;
    description?: string;
}