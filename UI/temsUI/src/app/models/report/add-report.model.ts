import { IOption } from './../option.model';
export interface IAddReportTemplate{
    id: string,
    name: string,
    description: string,
    subject: string, // equipment, room ...
    definitions: IOption[],
    personnel: IOption[],
    types: IOption[],
    rooms: IOption[],
    sepparateBy: string,
    commonProperties: string[],
    specificProperties: {
        type: string,
        properties: IOption[]
    }[],
    header: string,
    footer: string,
    signatories: IOption[],
}

export class AddReportTemplate implements IAddReportTemplate{
    id: string;
    name: string;
    description: string;
    subject: string;
    definitions: IOption[] = [];
    personnel: IOption[] = [];
    types: IOption[] = [];
    rooms: IOption[] = [];
    sepparateBy: string;
    commonProperties: string[];
    specificProperties: { type: string; properties: IOption[]; }[] = [];
    header: string;
    footer: string;
    signatories: IOption[] = [];
}