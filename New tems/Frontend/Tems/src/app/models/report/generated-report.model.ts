import { IOption } from './../option.model';

export class GeneratedReport{
    id: string;
    template: string;
    generatedBy: IOption;
    dateGenerated: Date;
}