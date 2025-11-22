import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { FieldWrapper, FormlyModule } from '@ngx-formly/core';

@Component({
  selector: 'app-select-tooltip',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatTooltipModule,
    MatIconModule,
    FormlyModule
  ],
  template: `
  <mat-form-field class="form-field">
    <mat-label>{{ field.templateOptions.label }}</mat-label>
    <mat-select [disabled]="field.templateOptions.disabled" [value]="field.templateOptions.value">
      <mat-option *ngFor="let option of $any(field.templateOptions.options)" [value]="option.id">
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
export class SelectTooltipComponent extends FieldWrapper implements OnInit {

  ngOnInit(): void {
  }
}