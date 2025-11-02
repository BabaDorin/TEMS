export interface ILoginModel{
    username: string,
    password: string,
}

export class LoginModel implements ILoginModel{
    username: string;
    password: string;
}