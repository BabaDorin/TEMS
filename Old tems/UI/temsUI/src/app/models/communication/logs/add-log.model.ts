import { IOption } from './../../option.model';

export interface IAddLog{
    addressees: IOption[],
    addresseesType: string;
    isImportant: boolean;
    text: string;
    logTypeId: string;
}

export class AddLog implements IAddLog{
    addressees: IOption[];
    addresseesType: string;
    isImportant: boolean;
    text: string;
    logTypeId: string;
}