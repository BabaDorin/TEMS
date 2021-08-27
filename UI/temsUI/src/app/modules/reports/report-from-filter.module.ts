import { ChipsAutocompleteModule } from './../chips-autocomplete/chips-autocomplete.module';
import { TranslateModule } from '@ngx-translate/core';
import { ReportPropertiesModule } from './report-properties.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatInputModule } from '@angular/material/input';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ReportFromFilterComponent } from './../../tems-components/reports/report-from-filter/report-from-filter.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    ReportFromFilterComponent
  ],
  imports: [
    CommonModule,
    MatIconModule,
    FormsModule,
    ReactiveFormsModule,
    MatInputModule,
    ReportPropertiesModule,
    MatButtonModule,
    TranslateModule,
    ChipsAutocompleteModule
  ],
  exports: [
    ReportFromFilterComponent
  ]
})
export class ReportFromFilterModule { }
