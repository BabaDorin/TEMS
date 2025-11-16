import { EquipmentFilter } from './../../helpers/filters/equipment.filter';
import { IOption } from './../option.model';
import { ITypeSpecificPropCollection } from './report.model';

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
    signatories: string[],
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
    specificProperties: ITypeSpecificPropCollection[] = [];
    properties?: string[] = [];
    header: string;
    footer: string;
    signatories: string[] = [];
}

export class ReportFromFilter {
    name: string;
    header: string;
    commonProperties: string[] = [];
    footer: string;
    signatories: string[] = [];
    filter: EquipmentFilter;
    
    // BEFREE: Add support for this thing (Might be useful in some use-cases)
    separateBy: string;
}