import { IOption } from './../option.model';

export interface IViewAllocationSimplified{
    id: string,
    equipment: IOption,
    room?: IOption;
    personnel?: IOption
    dateAllocated: Date;
    dateReturned?: Date;
}

export class ViewAllocationSimplified implements IViewAllocationSimplified{
    id: string;
    equipment: IOption;
    room?: IOption;
    personnel?: IOption;
    dateAllocated: Date;
    dateReturned?: Date;
}
