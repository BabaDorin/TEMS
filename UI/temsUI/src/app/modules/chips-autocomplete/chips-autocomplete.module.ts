import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';

@NgModule({
  declarations: [
    ChipsAutocompleteComponent
  ],
  imports: [
    CommonModule,
    MatInputModule,
    MatChipsModule,
    ReactiveFormsModule,
    FormsModule,
    MatIconModule,
    MatButtonModule,
    MatAutocompleteModule,
  ],
  exports:[
    ChipsAutocompleteComponent
  ]
})
export class ChipsAutocompleteModule { }
