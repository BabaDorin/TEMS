import { TranslateModule } from '@ngx-translate/core';
import { MatInputModule } from '@angular/material/input';
import { ChipsAutocompleteModule } from './../chips-autocomplete/chips-autocomplete.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { QuickAccessComponent } from './../../tems-components/equipment/quick-access/quick-access.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    QuickAccessComponent
  ],
  imports: [
    CommonModule,
    MatIconModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
    FormsModule,
    TranslateModule,
    ChipsAutocompleteModule
  ],
  exports: [
    QuickAccessComponent
  ]
})
export class QuickAccessModule { }
