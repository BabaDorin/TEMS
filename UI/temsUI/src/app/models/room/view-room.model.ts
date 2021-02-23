import { ViewPersonnelSimplified } from '../personnel/view-personnel-simplified.model';

// Used primarily for Details view
export interface IViewRooom{
    id: string,
    identifier: string,
    label: string,
    floor?: number,
    description?: string,
    supervisory?: ViewPersonnelSimplified[],
    photos?: string[];
    issueState: number; // 0 - ok (green), 1 - so-so, 2 (yellow) - not gud (red)
}

export class ViewRoom implements IViewRooom
{
    id: string;
    identifier: string;
    label: string;
    floor?: number;
    description?: string;
    supervisory?: ViewPersonnelSimplified[];
    photos?: string[];
    issueState: number;
    
    constructor(){
        this.id = '1';
        this.identifier = '307',
        this.label = 'depozit',
        this.floor = 3,
        this.description = '307 room description';
        this.supervisory = [ new ViewPersonnelSimplified() ];
        this.photos = ['smth'];
        this.issueState = 1;
    }
}