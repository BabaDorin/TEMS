import { FormGroup } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';

export interface IFormlyData{
    isVisible: boolean,
    model: any,
    form:  FormGroup;
    fields: FormlyFieldConfig[],
}

export class FormlyData implements IFormlyData{
    isVisible: boolean = false;
    model: any = {};
    form: FormGroup = new FormGroup({});
    fields: FormlyFieldConfig[] = [];

    wipeModel(){
        this.model = {};
    }

    getProp(propName: string){
        return this.model[propName]; 
    }
}
