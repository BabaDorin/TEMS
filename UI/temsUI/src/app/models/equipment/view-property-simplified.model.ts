export interface IViewPropertySimplified{
    id: string,
    displayName: string,
    description: string
}

export class ViewPropertySimplified implements IViewPropertySimplified{
    id: string;
    displayName: string;
    description: string;
}