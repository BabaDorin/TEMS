export interface IGenericContainerModel{
    title: string,
    tagGroups: ITagGroup[];
    actions: IContainerAction[];
    description: string;
    eventEmitted: Function;
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