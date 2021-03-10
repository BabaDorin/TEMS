import { IOption } from './../option.model';
export interface IAddRoom{
    identifier: string,
    floor: number,
    description: string,
    labels: IOption[],
}

export class AddRoom implements IAddRoom{
    identifier: string;
    floor: number;
    description: string;
    labels: IOption[];
}