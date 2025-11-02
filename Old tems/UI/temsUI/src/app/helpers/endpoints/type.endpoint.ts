import { TypeService } from './../../services/type.service';
import { Observable } from 'rxjs';
import { IOption } from 'src/app/models/option.model';
import { IOptionsEndpoint } from './../../models/form/options-endpoint.model';

export class TypeEndpoint implements IOptionsEndpoint {

    private _typeService: TypeService;
    public includeChildTypes: boolean = true;

    constructor(
        typeService: TypeService,
        includeChildTypes: boolean = true){
        this._typeService = typeService;
        this.includeChildTypes = includeChildTypes;
    }
    
    getOptions(): Observable<IOption[]>{
        return this._typeService.getAllAutocompleteOptions(undefined, this.includeChildTypes);
    }
}