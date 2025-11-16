export interface IEmailPreferencesModel {
    userId: string,
    email: string,
    getNotifications: boolean;
}
 
export class EmailPreferencesModel implements IEmailPreferencesModel{
    userId: string;
    email: string;
    getNotifications: boolean;
}