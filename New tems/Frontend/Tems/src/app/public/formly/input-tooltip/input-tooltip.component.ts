import { Component, OnInit } from '@angular/core';
import { FieldType } from '@ngx-formly/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'input-tooltip',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatTooltipModule,
    MatIconModule
  ],
  template: `
  <mat-form-field class="form-field">
    <mat-label>{{ field.templateOptions.label }}</mat-label>
    <input matInput [type]="field.templateOptions.type" [pattern]="field.templateOptions.pattern"
      [formControl]="$any(formControl)" [value]="field.defaultValue">
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
  }
}