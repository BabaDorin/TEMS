import { Component, OnInit } from '@angular/core';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'select-tooltip',
  template: `
  <mat-form-field class="form-field">
    <mat-label>{{ field.templateOptions.label }}</mat-label>
    <mat-select [disabled]="field.templateOptions.disabled" [value]="field.templateOptions.value">
      <mat-option *ngFor="let option of field.templateOptions.options" [value]="option.id">
        {{option.value}}
      </mat-option>
    </mat-select>
    <mat-icon matSuffix>
      <i class="mdi mdi-information-outline" *ngIf="field.templateOptions.description != undefined"
        [matTooltip]="field.templateOptions.description"></i>
    </mat-icon>
  </mat-form-field>
 `,
  styles: [`
    .form-field {
      width: 100%
    }
  `]
})
export class SelectTooltipComponent extends FieldType implements OnInit {

  ngOnInit(): void {
  }
}