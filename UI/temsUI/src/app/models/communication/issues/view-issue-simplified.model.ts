import { IOption } from './../../option.model';
import { ViewPersonnelSimplified } from "../../personnel/view-personnel-simplified.model";
import { ViewUserSimplified } from "../../user/view-user.model";

export interface IViewIssueSimplified{
    id: string;
    problem: string;
    status: string;
    description?: string;
    personnel?: IOption[];
    equipments?: IOption[];
    rooms?: IOption[];
    dateCreated: Date;
    dateClosed?: Date;
    closedBy?: ViewUserSimplified;
}

export class ViewIssueSimplified implements IViewIssueSimplified{
    id: string;
    problem: string;
    description?: string;
    status: string;
    personnel?: IOption[];
    equipments?: IOption[];
    rooms?: IOption[];
    dateCreated: Date;
    dateClosed?: Date;
    closedBy?: ViewUserSimplified;

    // constructor(){
    //     this.id = '1';
    //     this.problem = 'Este nevoie de interventia unui tehnolog';
    //     this.description = 'Nu avem conexiune la internet';
    //     this.status = 'urgent';
    //     let personnel = new ViewPersonnelSimplified();
    //     personnel.name = 'Arnold Schartzneger';
    //     this.personnel =  [ personnel ];
    //     this.dateCreated = new Date();
    // }
}