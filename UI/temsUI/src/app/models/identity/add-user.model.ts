import { IOption } from 'src/app/models/option.model';
export interface IAddUser{
    id?: string;
    username: string,
    password: string,
    fullName?: string,
    email?: string,
    phoneNumber?: string,
    personnel?: IOption,
    roles?: IOption[]
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
}