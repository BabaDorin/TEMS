import { IViewUserSimplified, ViewUserSimplified } from './../user/view-user.model';
export interface IViewReportSimplified{
    id: string;
    name: string;
    description?: string;
    createdBy?: IViewUserSimplified,
    isDefault?: boolean;
    dateCreated: Date;
}

export class ViewReportSimplified implements IViewReportSimplified{
    id: string;
    name: string;
    description?: string;
    createdBy?: ViewUserSimplified;
    isDefault?: boolean = true;
    dateCreated: Date;
    
    constructor(){
        this.id = '1';
        this.name = 'custom report';
        this.description = 'Description of the custom report';
        this.createdBy = new ViewUserSimplified();
        this.dateCreated = new Date();
    }
}