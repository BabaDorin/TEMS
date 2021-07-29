import { Observable } from 'rxjs';
import { IOption } from './../option.model';

export interface IOptionsEndpoint {
    getOptions() : Observable<IOption[]>;
    getOptions(input) : Observable<IOption[]>;
}