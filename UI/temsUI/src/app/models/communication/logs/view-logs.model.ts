import { IOption } from 'src/app/models/option.model';

export interface IViewLog {
    id: string;
    dateCreated: Date;
    text?: string;
    equipment?: IOption;
    room?: IOption;
    personnel?: IOption;
    logType: IOption;
    isImportant: boolean;
}

export class ViewLog implements IViewLog {
    id: string;
    dateCreated: Date;
    text?: string;
    equipment?: IOption;
    room?: IOption;
    personnel?: IOption;
    logType: IOption;
    isImportant: boolean;
}





