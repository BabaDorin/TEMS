import { IOption } from './../option.model';
export interface IAddAllocation {
    equipments: IOption[],
    allocateToType: string;
    allocateToId: string;
}

export class AddAllocation implements IAddAllocation{
    equipments: IOption[];
    allocateToType: string;
    allocateToId: string;
}