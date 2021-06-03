export interface IGenericContainerModel{
    title: string,
    tagGroups: ITagGroup[];
    actions: IContainerAction[];
    description: string;
}   

export interface ITagGroup{
    name: string,
    tags: string[];   
}

export interface IContainerAction{
    name: string,
    icon: string,
    action: Function
}