import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FieldArrayType, FormlyModule } from '@ngx-formly/core';

@Component({
  selector: 'app-repeat-type',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, FormlyModule],
  template: `
    <div *ngFor="let field of field.fieldGroup; let i = index;">
      <formly-field [field]="field"></formly-field>
      <button mat-button type="button" (click)="remove(i)">Remove</button>
    </div>
    <button mat-button type="button" (click)="add()">Add</button>
  `,
  styleUrls: ['./repeat-type.component.scss']
})
export class RepeatTypeComponent extends FieldArrayType {
}