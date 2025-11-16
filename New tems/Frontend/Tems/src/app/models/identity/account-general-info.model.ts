export interface IAccountGeneralInfoModel{
    userId: string,
    username: string,
    fullName: string,
    // description: string
}

export class AccountGeneralInfoModel implements IAccountGeneralInfoModel{
    userId: string;
    username: string;
    fullName: string;
}