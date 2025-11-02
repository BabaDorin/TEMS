import { Observable } from 'rxjs';
import { IOption } from './../option.model';

export interface IOptionsEndpoint {
    getOptions(input?: any) : Observable<IOption[]>;
}