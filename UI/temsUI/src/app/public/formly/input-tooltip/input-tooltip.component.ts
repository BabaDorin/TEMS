import { Component, OnInit } from '@angular/core';
import { FieldType } from '@ngx-formly/core';
import {TooltipPosition} from '@angular/material/tooltip';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'input-tooltip',
  template: `
  <mat-form-field class="form-field">
    <mat-label>{{ field.templateOptions.label }}</mat-label>
    <input matInput [type]="field.templateOptions.type" [pattern]="field.templateOptions.pattern"
      [formControl]="formControl" [formlyAttributes]="field" [value]="field.defaultValue">
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
export class InputTooltipComponent extends FieldType implements OnInit{
  ngOnInit(){
    if(this.field.templateOptions.type == undefined) this.field.templateOptions.type = "text";
    console.log(this.field.templateOptions.type);
  }
}