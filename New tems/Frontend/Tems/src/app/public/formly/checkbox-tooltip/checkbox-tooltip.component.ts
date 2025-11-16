import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { FieldType, FormlyModule } from '@ngx-formly/core';

@Component({
  selector: 'app-checkbox-tooltip',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCheckboxModule,
    MatTooltipModule,
    MatIconModule,
    FormlyModule
  ],
  templateUrl: './checkbox-tooltip.component.html',
  styleUrls: ['./checkbox-tooltip.component.scss']
})
export class CheckboxTooltipComponent extends FieldType {
  // ...existing code...
}
