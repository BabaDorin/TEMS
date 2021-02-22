import { ViewPersonnelSimplified } from './../personnel/view-personnel-simplified.model';

// Used primarily for Details view
export interface IViewRooom{
    id: string,
    identifier: string,
    floor?: number,
    description?: string,
    supervisory: ViewPersonnelSimplified[],
}