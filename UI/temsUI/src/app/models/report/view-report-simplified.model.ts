import { IOption } from 'src/app/models/option.model';
import { IViewUserSimplified, ViewUserSimplified } from './../user/view-user.model';
export interface IViewReportSimplified{
    id: string;
    name: string;
    description?: string;
    createdBy?: IOption[];
    isDefault?: boolean;
    dateCreated: Date;
}

export class ViewReportSimplified implements IViewReportSimplified{
    id: string;
    name: string;
    description?: string;
    createdBy?: IOption[];
    isDefault?: boolean = true;
    dateCreated: Date;
}