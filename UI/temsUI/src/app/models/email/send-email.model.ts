export interface ISendEmail{
    from: string,
    addressees: string[],
    subject: string,
    text: string;
}

export class SendEmail implements ISendEmail{
    from: string;
    addressees: string[];
    subject: string;
    text: string;

    validate(): boolean{
        return this.from != undefined && 
                this.from.length > 0 &&
                this.addressees != undefined &&
                this.addressees.length > 0 &&
                this.subject != undefined &&
                this.subject.length > 0 && 
                this.text != undefined &&
                this.text.length > 0 
    }
}