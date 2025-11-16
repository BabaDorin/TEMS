import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule } from '@ngx-translate/core';
import { QuickAccessComponent } from './../../tems-components/equipment/quick-access/quick-access.component';
import { ChipsAutocompleteModule } from './../chips-autocomplete/chips-autocomplete.module';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MatIconModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
    FormsModule,
    TranslateModule,
    ChipsAutocompleteModule,
    QuickAccessComponent
  ],
  exports: [
    QuickAccessComponent
  ]
})
export class QuickAccessModule { }
