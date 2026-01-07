import { IOption } from './../option.model';
export interface IViewProfile{
    id: string,
    personnelId: string,
    fullName: string,
    firstName?: string,
    lastName?: string,
    username: string,
    phoneNumber: string,
    email: string,
    isArchieved: string,
    dateArchieved: Date,
    dateRegistered: Date,
    allocatedKey: IOption;
    roles: string[];
    getEmailNotifications: boolean,
    photoBase64: string;
}

export class ViewProfile implements IViewProfile{
    id: string;
    personnelId: string;
    fullName: string;
    firstName?: string;
    lastName?: string;
    username: string;
    phoneNumber: string;
    email: string;
    isArchieved: string;
    dateArchieved: Date;
    dateRegistered: Date;
    allocatedKey: IOption;
    roles: string[];
    getEmailNotifications: boolean;
    photoBase64: string;
}