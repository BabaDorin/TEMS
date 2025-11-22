export interface IChangePasswordModel{
    userId: string,
    oldPass: string,
    newPass: string,
    confirmNewPass: string,
}

export class ChangePasswordModel implements IChangePasswordModel{
    userId: string;
    oldPass: string;
    newPass: string;
    confirmNewPass: string;
}