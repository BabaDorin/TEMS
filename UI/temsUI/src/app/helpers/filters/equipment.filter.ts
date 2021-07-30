import { HttpParams } from "@angular/common/http";
import { BaseFilter } from "./base.filter";

export class EquipmentFilter extends BaseFilter {
    includeDerived: boolean = false;
    rooms: string[];
    personnel: string[];
    types: string[];

    getParams(): HttpParams {
        let params = new HttpParams();

        params = params.append('onlyParent', String(!this.includeDerived));

        params = this.listToParams(this.rooms, params, "rooms");
        params = this.listToParams(this.personnel, params, "personnel");
        params = this.listToParams(this.types, params, "types");

        return params;
    }
}