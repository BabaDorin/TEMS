import { EventEmitter } from '@angular/core';
export interface IGenericContainerModel{
    title: string,
    tagGroups: ITagGroup[];
    actions: IContainerAction[];
    description: string;

    // The following function is called when there is some data to be sent from container model as a container's 
    // output event. Call this function from container model, pass the data the following way: eventEmitted(eventData)
    // The genericContainerComponent will emmit an output event having the specified parameters that can be caught
    // via (eventEmitted) on app-generic-container.  
    eventEmitted: Function;
}   

export interface ITagGroup{
    name: string,
    tags: string[];   
}

export interface IContainerAction{
    name: string,
    icon: string,
    action?: Function,
    disabled?: boolean,
    actionParameters?: any;
}