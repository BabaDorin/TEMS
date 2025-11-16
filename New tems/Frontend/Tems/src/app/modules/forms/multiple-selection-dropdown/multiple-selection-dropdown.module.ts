import { MatOptionModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MultipleSelectionDropdownComponent } from './../../../shared/forms/multiple-selection-dropdown/multiple-selection-dropdown.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';



@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    MultipleSelectionDropdownComponent,
    ReactiveFormsModule,
    MatInputModule,
    MatSelectModule,
    MatFormFieldModule,
    MatOptionModule,
    FormsModule,
  ],
  exports: [
    MultipleSelectionDropdownComponent
  ]
})
export class MultipleSelectionDropdownModule { }
