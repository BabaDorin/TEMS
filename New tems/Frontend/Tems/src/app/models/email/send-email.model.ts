export interface ISendEmail{
    from: string,
    recipients: string[],
    subject: string,
    text: string;
}

export class SendEmail implements ISendEmail{
    from: string;
    recipients: string[];
    subject: string;
    text: string;

    validate(): boolean{
        return this.from != undefined && 
                this.from.length > 0 &&
                this.recipients != undefined &&
                this.recipients.length > 0 &&
                this.subject != undefined &&
                this.subject.length > 0 && 
                this.text != undefined &&
                this.text.length > 0 
    }
}