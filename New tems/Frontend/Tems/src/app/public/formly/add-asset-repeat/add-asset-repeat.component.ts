import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FieldArrayType, FormlyModule } from '@ngx-formly/core';

@Component({
  selector: 'app-add-asset-repeat',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, FormlyModule],
  template: `
    <div *ngFor="let field of field.fieldGroup; let i = index;">
      <formly-field [field]="field"></formly-field>
      <button mat-icon-button type="button" (click)="remove(i)">
        <mat-icon>delete</mat-icon>
      </button>
    </div>
    <button mat-raised-button type="button" (click)="add()">
      <mat-icon>add</mat-icon> Add Asset
    </button>
  `,
  styleUrls: ['./add-asset-repeat.component.scss']
})
export class AddAssetRepeatComponent extends FieldArrayType {
}