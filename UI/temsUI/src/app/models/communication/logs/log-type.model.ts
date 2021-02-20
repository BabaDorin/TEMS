export interface ILogType{
    id: string;
    type: string;
}

export class LogType implements ILogType{
    id: string;
    type: string;
    constructor(){
        this.id = '1';
        this.type = 'Simple log'
    }
}
    