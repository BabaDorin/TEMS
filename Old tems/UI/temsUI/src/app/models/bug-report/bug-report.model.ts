import { IOption } from './../option.model';
export class BugReport{
    reportType: string;
    description: string;
    attachments: File[] = [];
}

export class ViewBugReport {
    id: string;
    createdBy: IOption;
    dateCreated: Date;
    reportType: string;
    description: string;
    attachments: string[]; // contains only file names
}