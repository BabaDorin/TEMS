import { IOption } from './../option.model';
import { ViewUserSimplified } from './../user/view-user.model';
export interface IViewLibraryItem {
    id?: string,
    actualName: string,
    displayName: string,
    description?: string,
    dateUploaded: Date,
    uploadedBy?: IOption,
    dbPath: string,
    fileSize: number,
    downloads: number,
}

export class ViewLibraryItem implements IViewLibraryItem{
    id?: string;
    actualName: string;
    displayName: string;
    description?: string;
    dateUploaded: Date;
    uploadedBy?: IOption;
    dbPath: string;
    fileSize: number;
    downloads: number;
}