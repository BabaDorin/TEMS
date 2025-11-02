import { IOption } from 'src/app/models/option.model';
import { ViewUserSimplified } from '../user/view-user.model';
import { ViewPersonnelSimplified } from '../personnel/view-personnel-simplified.model';
import { ViewKeySimplified } from "./view-key.model";

export interface IViewKeyAllocation{
    id: string,
    key: IOption,
    personnel: IOption,
    room?: IOption,
    dateAllocated: Date,
    allocatedBy?: IOption,
    dateReturned?: Date,
}

export class ViewKeyAllocation implements IViewKeyAllocation{
    id: string;
    key: IOption;
    personnel: IOption;
    room?: IOption;
    dateAllocated: Date;
    allocatedBy?: IOption;
    dateReturned?: Date;
}