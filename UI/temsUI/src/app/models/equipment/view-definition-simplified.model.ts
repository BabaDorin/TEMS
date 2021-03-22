export interface IViewDefinitionSimplified{
    id: string,
    identifier: string,
    equipmentType: string,
    parent: string,
    children: string
}

export class ViewDefinitionSimplified implements IViewDefinitionSimplified{
    id: string;
    identifier: string;
    equipmentType: string;
    parent: string;
    children: string;
}