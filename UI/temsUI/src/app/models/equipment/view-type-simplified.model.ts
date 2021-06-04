export interface IViewTypeSimplified{
    id: string,
    name: string,
    properties: string,
    parents: string[],
    children: string[]
}

export class ViewTypeSimplified implements IViewTypeSimplified{
    id: string;
    name: string;
    properties: string;
    parents: string[];
    children: string[]
}