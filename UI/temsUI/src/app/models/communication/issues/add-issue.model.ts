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

    // for testing purposes
    // constructor(){
    //     this.problem = '';
    //     this.problemDescription = '';
    //     this.roomIdentifier = [] as IOption[];
    //     this.equipmentIdentifier = [] as IOption[];
    //     this.personnelIdentifier = [] as IOption[];
    //     this.status = 'medium';
    //     this.asignees = [] as IOption[];
    //     this.createdBy = {} as IOption;
    // }
}