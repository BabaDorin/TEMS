import { IOption } from 'src/app/models/option.model';
export interface IAddUser{
    username: string,
    password: string,
    fullName?: string,
    email?: string,
    phoneNumber?: string,
    personnel?: IOption,
    roles?: IOption[]
}

export class AddUser implements IAddUser{
    username: string;
    password: string;
    fullName?: string;
    email?: string;
    phoneNumber?: string;
    personnel?: IOption;
    roles?: IOption[];
}