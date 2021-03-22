export interface IViewTypeSimplified{
    id: string,
    name: string,
    properties: string,
    parent: string,
    childrent: string
}

export class ViewTypeSiplified implements IViewTypeSimplified{
    id: string;
    name: string;
    properties: string;
    parent: string;
    childrent: string;
}