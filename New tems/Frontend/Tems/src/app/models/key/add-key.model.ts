import { IOption } from 'src/app/models/option.model';

export interface IAddKey{
  identifier: string,
  numberOfCopies?: number,
  roomId?: IOption,
  descriptions?: string;  
}

export class AddKey implements IAddKey{
    identifier: string;
    numberOfCopies?: number = 0;
    roomId?: IOption;
    descriptions?: string;
}