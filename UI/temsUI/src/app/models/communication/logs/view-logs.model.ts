import { IOption } from 'src/app/models/option.model';

export class ViewLog {
    id: string;
    dateCreated: Date;
    createdBy: IOption;
    text: string;
    equipment: IOption;
    room: IOption;
    personnel?: IOption;
    logType: IOption;
    isImportant: boolean;
}





