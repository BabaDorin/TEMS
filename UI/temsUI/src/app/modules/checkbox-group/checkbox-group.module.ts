import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CheckboxGroupComponent } from './../../shared/forms/checkbox-group/checkbox-group.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    CheckboxGroupComponent
  ],
  imports: [
    CommonModule,
    MatTooltipModule,
    MatCheckboxModule,
    MatIconModule,
    FormsModule,
  ],
  exports: [
    CheckboxGroupComponent
  ]
})
export class CheckboxGroupModule { }
