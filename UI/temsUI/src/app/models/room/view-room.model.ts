import { IOption } from './../option.model';
import { ViewPersonnelSimplified } from '../personnel/view-personnel-simplified.model';

// Used primarily for Details view
export interface IViewRooom{
    id: string,
    identifier: string,
    floor?: number,
    description?: string,
    supervisory?: ViewPersonnelSimplified[],
    photos?: string[];
    activeTickets: number; // 0 - ok (green), 1 - so-so, 2 (yellow) - not gud (red)
    labels?: IOption[],
}

export class ViewRoom implements IViewRooom
{
    id: string;
    identifier: string;
    floor?: number;
    description?: string;
    supervisory?: ViewPersonnelSimplified[];
    photos?: string[];
    activeTickets: number;
    labels?: IOption[];
}