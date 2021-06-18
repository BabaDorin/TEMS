import { IOption } from './../option.model';
export interface IViewUserSimplified{
    id: string,
    username: string,
    fullName: string,
    phoneNumber: string,
    email: string,
    roles: string[]
}

export class ViewUserSimplified implements IViewUserSimplified{
    id: string;
    username: string;
    fullName: string;
    phoneNumber: string;
    email: string;
    roles: string[];
}

export interface IViewUser{
    id: string,
    username: string,
    email: string,
    phoneNumber: string,
    fullName: string,
    personnel: IOption,
    roles: string[],
    claims: string[];
}

export class ViewUser implements IViewUser{
    id: string;
    username: string;
    email: string;
    phoneNumber: string;
    fullName: string;
    personnel: IOption;
    roles: string[];
    claims: string[];
}