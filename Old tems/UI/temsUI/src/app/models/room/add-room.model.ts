import { IOption } from './../option.model';
export interface IAddRoom{
    id?: string,
    identifier: string,
    floor: number,
    description: string,
    labels: IOption[],
    supervisories: IOption[]
}

export class AddRoom implements IAddRoom{
    id?: string;
    identifier: string;
    floor: number;
    description: string;
    labels: IOption[];
    supervisories: IOption[];
}