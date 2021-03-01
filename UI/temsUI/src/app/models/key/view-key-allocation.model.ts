import { ViewUserSimplified } from '../user/view-user.model';
import { ViewPersonnelSimplified } from '../personnel/view-personnel-simplified.model';
import { ViewKeySimplified } from "./view-key.model";

export interface IViewKeyAllocation{
    key: ViewKeySimplified,
    personnel: ViewPersonnelSimplified,
    dateAllocated: Date,
    allocatedBy?: ViewUserSimplified,
    returnedDate?: Date,
}

export class ViewKeyAllocation implements IViewKeyAllocation{
    key: ViewKeySimplified;
    personnel: ViewPersonnelSimplified;
    dateAllocated: Date;
    allocatedBy?: ViewUserSimplified;
    returnedDate?: Date;

    constructor(){
        this.key = new ViewKeySimplified();
        this.personnel = new ViewPersonnelSimplified();
        this.dateAllocated = new Date();
        this.allocatedBy = new ViewUserSimplified();
        this.returnedDate = new Date(); 
    }
}