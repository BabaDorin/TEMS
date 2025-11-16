export interface IViewRole{
    id: string;
    name: string;
    claims: string[];
}

export class ViewRole implements IViewRole{
    id: string;
    name: string;
    claims: string[];
}