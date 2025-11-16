import { TEMS_FORMS_IMPORTS } from './../../tems-forms/tems-forms.module';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatInputModule } from '@angular/material/input';
import { FormlyModule } from '@ngx-formly/core';
import { TranslateModule } from '@ngx-translate/core';
import { SendEmailComponent } from 'src/app/tems-components/send-email/send-email.component';
import { ChipsAutocompleteModule } from './../../chips-autocomplete/chips-autocomplete.module';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    SendEmailComponent,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
    ...TEMS_FORMS_IMPORTS,
    FormsModule,
    ChipsAutocompleteModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    TranslateModule,
    MatChipsModule,
  ],
  exports:[
    SendEmailComponent
  ]
})
export class EmailModule { }
