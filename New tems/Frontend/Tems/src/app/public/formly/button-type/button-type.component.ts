import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FieldType, FormlyModule } from '@ngx-formly/core';

@Component({
  selector: 'app-button-type',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, FormlyModule],
  template: `<button mat-raised-button type="button" (click)="onClick($event)">{{to.label}}</button>`,
  styleUrls: ['./button-type.component.scss']
})
export class ButtonTypeComponent extends FieldType {
  onClick($event) {
    if (this.to.onClick) {
      this.to.onClick($event);
    }
  }
}