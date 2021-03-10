import { IOption } from './../option.model';
interface IAddPersonnel {
    name: string,
    phoneNumber?: string,
    email?: string;
    positions?: IOption[];
}

export class AddPersonnel implements IAddPersonnel{
    name: string;
    phoneNumber?: string;
    email?: string;
    positions?: IOption[];
}