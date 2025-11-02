import { IOption } from './../option.model';
export interface IAddUser{
    id?: string;
    username: string,
    password: string,
    fullName?: string,
    email?: string,
    phoneNumber?: string,
    personnel?: IOption,
    roles?: IOption[],
    claims?: IOption[],
}

export class AddUser implements IAddUser{
    id?: string;
    username: string;
    password: string;
    fullName?: string;
    email?: string;
    phoneNumber?: string;
    personnel?: IOption;
    roles?: IOption[];
    claims?: IOption[];
}