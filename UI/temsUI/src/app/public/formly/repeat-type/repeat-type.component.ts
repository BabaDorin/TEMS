import { Component } from '@angular/core';
import { FieldArrayType, FormlyFieldConfig } from '@ngx-formly/core';

@Component({
  selector: 'formly-repeat-section',
  template:`
  <div *ngFor="let field of field.fieldGroup; let i = index;">
      <formly-group
        [model]="model[i]"
        [field]="field"
        [options]="options"
        [form]="formControl">
        <div class="col-sm-2 d-flex align-items-center">
          <button class="btn btn-danger" type="button" (click)="remove(i)">Remove</button>
        </div>
      </formly-group>
    </div>
    <div style="margin:30px 0;">
      <button class="btn btn-primary" type="button" (click)="add()">{{field.fieldArray.templateOptions.btnText}}</button>
    </div>
`,
  styles: [`
    :host {
      border: 1px solid;
      display: block;
      padding: 5px;
    }
  `]
})
export class RepeatTypeComponent extends FieldArrayType {
}