export interface IViewPersonnelSimplified{
    id: string;
    name: string;
    allocatedEquipment?: number; // Number of allocated equipment items
    pozition?: string, // professor, auxiliary employee etc...
    issues?: number; // number of opened issues by this personnel
}

export class ViewPersonnelSimplified implements IViewPersonnelSimplified{
    id: string;
    name: string;
    allocatedEquipment?: number; // Number of allocated equipment items
    pozition?: string; // professor, auxiliary employee etc...
    issues?: number; // number of opened issues by this personnel

    constructor(){
        this.id = '1';
        this.name = 'Baba Dory';
        this.pozition = 'Professor';
        this.issues = 3;
        this.allocatedEquipment = 1;
    }
}