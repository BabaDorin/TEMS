import { IOption } from './../option.model';
export interface IViewProfile{
    id: string,
    personnelId: string,
    fullName: string,
    username: string,
    phoneNumber: string,
    email: string,
    isArchieved: string,
    dateArchieved: Date,
    dateRegistered: Date,
    allocatedKey: IOption;
}

export class ViewProfile implements IViewProfile{
    id: string;
    personnelId: string;
    fullName: string;
    username: string;
    phoneNumber: string;
    email: string;
    isArchieved: string;
    dateArchieved: Date;
    dateRegistered: Date;
    allocatedKey: IOption;
}