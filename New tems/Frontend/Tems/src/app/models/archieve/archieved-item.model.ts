import { IOption } from 'src/app/models/option.model';
export interface IArchievedItem {
    id: string,
    identifier: string,
    dateArchieved: Date,
    archievedBy: IOption,
}

export class ArchievedItem implements IArchievedItem {
    id: string;
    identifier: string;
    dateArchieved: Date;
    archievedBy: IOption;
}