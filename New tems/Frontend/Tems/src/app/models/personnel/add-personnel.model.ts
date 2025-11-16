import { IOption } from './../option.model';

interface IAddPersonnel {
    id?: string,
    name: string,
    phoneNumber?: string,
    email?: string;
    positions?: IOption[];
    user?: IOption;
}

export class AddPersonnel implements IAddPersonnel{
    id?: string;
    name: string;
    phoneNumber?: string;
    email?: string;
    positions?: IOption[];
    user?: IOption;
}