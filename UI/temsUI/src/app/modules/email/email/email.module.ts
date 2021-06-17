import { SendEmailComponent } from 'src/app/tems-components/send-email/send-email.component';
import { ChipsAutocompleteModule } from './../../chips-autocomplete/chips-autocomplete.module';
import { MatChipsModule } from '@angular/material/chips';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { FormlyModule } from '@ngx-formly/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    SendEmailComponent,
  ],
  imports: [
    CommonModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
    FormlyModule,
    FormsModule,
    ChipsAutocompleteModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatChipsModule,
  ],
  exports:[
    SendEmailComponent
  ]
})
export class EmailModule { }
