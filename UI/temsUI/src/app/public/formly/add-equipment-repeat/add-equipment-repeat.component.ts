import { Component } from '@angular/core';
import { FieldArrayType, FormlyFieldConfig } from '@ngx-formly/core';

@Component({
  selector: 'add-eq-repeat-section',
  template: `
  <div *ngFor="let field of field.fieldGroup; let i = index;">
        <div style="margin-bottom:5px;">
          <button class="btn btn-danger btn-sm" type="button" (click)="remove(i)">Remove</button>
        </div>
      <formly-group
        [model]="model[i]"
        [field]="field"
        [options]="options"
        [form]="formControl">
      </formly-group>
    </div>
    <div style="margin:10px 0;">
      <button class="btn btn-secondary btn-sm" type="button" (click)="add()">{{field.fieldArray.templateOptions.btnText}}</button>
    </div>
`,
})
export class AddEquipmentRepeatComponent extends FieldArrayType {
}