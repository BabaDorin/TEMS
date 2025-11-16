export interface IResponse{
    message: string;
    status: number;
    additional?: any;
}

export class Response implements IResponse {
    message: string;
    status: number;
    additional?: any;

    constructor(msg: string, sts: number, addit?: any){
        this.message = msg;
        this.status = sts;
        this.additional = addit;
    }
}

export class ResponseFactory {
    static Fail(message: string, additional?:any) : IResponse {
        return new Response(message, 0, additional);
    }

    static Success(message: string, additional?:any) : IResponse {
        return new Response(message, 1, additional);
    }

    static Neutral(message: string, additional?:any) : IResponse {
        return new Response(message, 2, additional);
    }
}