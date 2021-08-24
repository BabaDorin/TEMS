import { forEach } from '@angular-devkit/schematics';
import { IOption } from './../option.model';
export interface IAddReportTemplate{
    id: string,
    name: string,
    description: string,
    definitions: IOption[],
    personnel: IOption[],
    types: IOption[],
    rooms: IOption[],
    separateBy: string,
    includeInUse: boolean,
    includeUnused: boolean,
    includeFunctional: boolean,
    includeDefect: boolean,
    includeParent: boolean,
    includeChildren: boolean,
    properties?: string[],
    header: string,
    footer: string,
    signatories: IOption[],
}

export class AddReportTemplate implements IAddReportTemplate{
    id: string;
    name: string;
    description: string;
    definitions: IOption[] = [];
    personnel: IOption[] = [];
    types: IOption[] = [];
    rooms: IOption[] = [];
    separateBy: string;
    includeInUse: boolean;
    includeUnused: boolean;
    includeFunctional: boolean;
    includeDefect: boolean;
    includeParent: boolean;
    includeChildren: boolean;
    commonProperties: string[];
    specificProperties: { type: string; properties: string[]; }[] = [];
    properties?: string[] = [];
    header: string;
    footer: string;
    signatories: IOption[] = [];
}