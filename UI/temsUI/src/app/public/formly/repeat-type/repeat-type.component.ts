import { Component } from '@angular/core';
import { FieldArrayType, FormlyFieldConfig } from '@ngx-formly/core';

@Component({
  selector: 'formly-repeat-section',
  template: `
  <div *ngFor="let field of field.fieldGroup; let i = index;">
        <div style="margin-bottom:5px;">
          <button type="button" mat-button (click)="remove(i)">
            <mat-icon class="text-danger">remove</mat-icon>
            Remove
          </button>
        </div>
      <formly-group
        [model]="model[i]"
        [field]="field"
        [options]="options"
        [form]="formControl">
      </formly-group>
    </div>
    <div style="margin:10px 0;">
      <button type="button" mat-button (click)="add()">
        <mat-icon class="text-primary">add</mat-icon>
        {{field.fieldArray.templateOptions.btnText}}
      </button>
    </div>
`,
})
export class RepeatTypeComponent extends FieldArrayType {
}