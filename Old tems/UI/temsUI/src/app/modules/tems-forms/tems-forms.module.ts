import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatOptionModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormlyModule } from '@ngx-formly/core';
import { FormlyMaterialModule } from '@ngx-formly/material';
import { TranslateModule } from '@ngx-translate/core';
import { TagInputModule } from 'ngx-chips';
import { fieldMatchValidator, specCharValidator, usernameValidator } from 'src/app/models/validators';
import { AddEquipmentRepeatComponent } from './../../public/formly/add-equipment-repeat/add-equipment-repeat.component';
import { AutocompleteTypeComponent } from './../../public/formly/autocomplete-type/autocomplete-type.component';
import { ButtonTypeComponent } from './../../public/formly/button-type/button-type.component';
import { FormlyWrapperComponent } from './../../public/formly/formly-wrapper/formly-wrapper.component';
import { InputTooltipComponent } from './../../public/formly/input-tooltip/input-tooltip.component';
import { RepeatTypeComponent } from './../../public/formly/repeat-type/repeat-type.component';
import { SelectTooltipComponent } from './../../public/formly/select-tooltip/select-tooltip.component';
import { CheckboxGroupModule } from './../checkbox-group/checkbox-group.module';
import { ChipsAutocompleteModule } from './../chips-autocomplete/chips-autocomplete.module';

@NgModule({
  declarations: [
    InputTooltipComponent,
    SelectTooltipComponent,
    FormlyWrapperComponent,
    AutocompleteTypeComponent,
    ButtonTypeComponent,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
    RepeatTypeComponent,
    AddEquipmentRepeatComponent,
  ],
  imports: [
    CommonModule,
    MatInputModule,
    MatAutocompleteModule,
    ReactiveFormsModule,
    MatOptionModule,
    MatFormFieldModule,
    TranslateModule,
    MatTooltipModule,
    CheckboxGroupModule,
    MatCheckboxModule,
    MatIconModule,
    MatSelectModule,
    ChipsAutocompleteModule,
    FormlyModule.forRoot({
      wrappers: [
        { name: 'formly-wrapper', component: FormlyWrapperComponent },
      ],
      types: [
        {
          name: 'input-tooltip',
          component: InputTooltipComponent,
          defaultOptions: {
            type: 'text'
          }
        },
        {
          name: 'select-tooltip',
          component: SelectTooltipComponent,
        },
        {
          name: 'autocomplete',
          component: AutocompleteTypeComponent,
          wrappers: ['form-field'],
        },
        {
          name: 'button',
          component: ButtonTypeComponent,
          wrappers: ['form-field'],
          defaultOptions: {
            templateOptions: {
              btnType: 'default',
              type: 'button',
            },
          },
        },
        { name: 'repeat', component: RepeatTypeComponent },
        { name: 'eq-repeat', component: AddEquipmentRepeatComponent },
      ],
      validators: [
        { name: 'specCharValidator', validation: specCharValidator },
        { name: 'usernameValidator', validation: usernameValidator },
        { name: 'fieldMatch', validation: fieldMatchValidator },
      ],  
      validationMessages: [
        { name: 'specCharValidator', message: 'No special characters or spaces allowed!' },
      ]
    }),
  ],
  exports: [
    CommonModule,
    
    // Angular form modules
    FormsModule,
    ReactiveFormsModule,
    
    // Formly modules
    FormlyModule,
    FormlyMaterialModule,

    // Other
    CheckboxGroupModule,
    TagInputModule,
    ChipsAutocompleteModule,
  ]
})
export class TemsFormsModule { }
