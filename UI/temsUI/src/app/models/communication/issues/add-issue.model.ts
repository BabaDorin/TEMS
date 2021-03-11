import { IOption } from '../../option.model';
export interface IAddIssue{
    problem: string;
    problemDescription?: string,
    status: string,
    rooms?: IOption[],    
    equipments?: IOption[],    
    personnel?: IOption[]
    asignees?: IOption[];
    createdBy?: IOption;
}

export class AddIssue implements IAddIssue{
    problem: string;
    problemDescription?: string;
    rooms?: IOption[];
    equipments?: IOption[];
    personnel?: IOption[];
    status: string;
    asignees?: IOption[];
    createdBy?: IOption;
}