import { IOption } from './../option.model';
export interface IAddRoom{
    id?: string,
    identifier: string,
    floor: number,
    description: string,
    labels: IOption[],
}

export class AddRoom implements IAddRoom{
    id?: string;
    identifier: string;
    floor: number;
    description: string;
    labels: IOption[];
}