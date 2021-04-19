export interface ISICFileUploadResult {
    fileName: string;
    status: number;
    message: string;
    ellapsedMiliseconds: number;
}

export class SICFileUploadResult implements ISICFileUploadResult{
    fileName: string;
    status: number;
    message: string;
    ellapsedMiliseconds: number;
}