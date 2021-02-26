import { ViewUserSimplified } from './../user/view-user.model';
export interface IViewLibraryItem {
    id?: string,
    name: string;
    description?: string;
    dateUploaded: Date,
    dateRemoved?: Date
    uploadedBy: ViewUserSimplified,
    removedBy?: ViewUserSimplified,
    imagePath?: string,
    downloadLink?: string,
}

export class ViewLibraryItem implements IViewLibraryItem{
    id?: string;
    name: string;
    description?: string;
    dateUploaded: Date;
    dateRemoved?: Date;
    uploadedBy: ViewUserSimplified;
    removedBy?: ViewUserSimplified;
    imagePath?: string;
    
    constructor(){
        this.id = '1';
        this.name = 'Team Viewer',
        this.description = 'TV Description',
        this.dateUploaded = new Date();
        this.uploadedBy = new ViewUserSimplified();
    }
}