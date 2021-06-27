import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { ChipsAutocompleteModule } from './../chips-autocomplete/chips-autocomplete.module';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { SelectTooltipComponent } from './../../public/formly/select-tooltip/select-tooltip.component';
import { TagInputModule } from 'ngx-chips';
import { AddEquipmentRepeatComponent } from './../../public/formly/add-equipment-repeat/add-equipment-repeat.component';
import { RepeatTypeComponent } from './../../public/formly/repeat-type/repeat-type.component';
import { ButtonTypeComponent } from './../../public/formly/button-type/button-type.component';
import { AutocompleteTypeComponent } from './../../public/formly/autocomplete-type/autocomplete-type.component';
import { FormlyWrapperComponent } from './../../public/formly/formly-wrapper/formly-wrapper.component';
import { InputTooltipComponent } from './../../public/formly/input-tooltip/input-tooltip.component';
import { FormlyMaterialModule } from '@ngx-formly/material';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FormlyModule } from '@ngx-formly/core';
import { MaterialModule } from '../material/material.module';
import { fieldMatchValidator, specCharValidator } from 'src/app/models/validators';
import { usernameValidator } from 'src/app/models/validators';
import { CheckboxGroupComponent } from 'src/app/shared/forms/checkbox-group/checkbox-group.component';
import { MatOptionModule } from '@angular/material/core';

@NgModule({
  declarations: [
    InputTooltipComponent,
    SelectTooltipComponent,
    FormlyWrapperComponent,
    AutocompleteTypeComponent,
    ButtonTypeComponent,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
    RepeatTypeComponent,
    AddEquipmentRepeatComponent,
    CheckboxGroupComponent
  ],
  imports: [
    CommonModule,
    MatInputModule,
    MatAutocompleteModule,
    ReactiveFormsModule,
    MatOptionModule,
    MatFormFieldModule,
    MatTooltipModule,
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
    MaterialModule,
    TagInputModule,
    CheckboxGroupComponent,
    ChipsAutocompleteModule,
  ]
})
export class TemsFormsModule { }
