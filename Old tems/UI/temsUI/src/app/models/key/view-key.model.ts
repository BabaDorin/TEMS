import { IOption } from 'src/app/models/option.model';
export interface IViewKeySimplified{
    id: string;
    identifier: string;
    room: IOption;
    allocatedTo: IOption;
    timePassed?: string;
    description?: string;
}

export class ViewKeySimplified implements IViewKeySimplified{
    id: string;
    identifier: string;
    room: IOption;
    allocatedTo: IOption;
    timePassed?: string;
    description?: string;
}