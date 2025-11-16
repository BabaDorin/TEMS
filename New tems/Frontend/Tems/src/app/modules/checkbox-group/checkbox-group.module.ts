import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CheckboxGroupComponent } from './../../shared/forms/checkbox-group/checkbox-group.component';

@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    CheckboxGroupComponent,
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
