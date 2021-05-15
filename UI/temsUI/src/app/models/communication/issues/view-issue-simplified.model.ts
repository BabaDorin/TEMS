import { IOption } from 'src/app/models/option.model';
import { ViewPersonnelSimplified } from "../../personnel/view-personnel-simplified.model";
import { ViewUserSimplified } from "../../user/view-user.model";

export interface IViewIssueSimplified{
    id: string;
    problem: string;
    status?: IOption;
    isPinned: boolean;
    label?: IOption;
    description?: string;
    personnel?: IOption[];
    equipments?: IOption[];
    rooms?: IOption[];
    dateCreated: Date;
    dateClosed?: Date;
    closedBy?: IOption;
    assignees?: IOption[]
}

export class ViewIssueSimplified implements IViewIssueSimplified{
    id: string;
    problem: string;
    isPinned: boolean;
    description?: string;
    status?: IOption;
    label?: IOption;
    personnel?: IOption[];
    equipments?: IOption[];
    rooms?: IOption[];
    dateCreated: Date;
    dateClosed?: Date;
    closedBy?: IOption;
    assignees?: IOption[]
}