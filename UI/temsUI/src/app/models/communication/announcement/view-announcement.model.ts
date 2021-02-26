import { ViewUserSimplified } from './../../user/view-user.model';
export interface IViewAnnouncement{
    id?: string;
    title: string;
    text: string;
    createdBy?: ViewUserSimplified;
    dateCreated: Date;
}

export class ViewAnnouncement implements IViewAnnouncement{
    id?: string;
    title: string;
    text: string;
    createdBy?: ViewUserSimplified;
    dateCreated: Date;

    constructor(){
        this.id = '1';
        this.title = 'Announcement title';
        this.text = 'Announcement text';
        this.createdBy = new ViewUserSimplified();
        this.dateCreated = new Date();
    }
}