import { IOption } from 'src/app/models/option.model';
import { ViewUserSimplified } from './../../user/view-user.model';
export interface IViewAnnouncement{
    id?: string;
    title: string;
    text: string;
    createdBy?: IOption;
    dateCreated: Date;
}

export class ViewAnnouncement implements IViewAnnouncement{
    id?: string;
    title: string;
    text: string;
    createdBy?: IOption;
    dateCreated: Date;
}