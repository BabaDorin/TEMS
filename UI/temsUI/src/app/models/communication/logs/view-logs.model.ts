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

    constructor() {
        this.id = '1';
        this.dateCreated = new Date();
        this.text = 'Equipment has been repaired';
        this.logType = { value: '1', label: 'Simple Log Type' };
        this.isImportant = true;
    }
}





