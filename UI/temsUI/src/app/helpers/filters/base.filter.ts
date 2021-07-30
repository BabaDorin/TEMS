import { HttpParams } from '@angular/common/http';

// Base class for any filter
// Contains some helper methods
export abstract class BaseFilter {

    protected listToParams(list: any[], params: HttpParams, paramsPropertyName: string){
        if (list != undefined)
            list.forEach(element => {
                params = params.append(paramsPropertyName, element);
            });
        
        return params;
    }
}