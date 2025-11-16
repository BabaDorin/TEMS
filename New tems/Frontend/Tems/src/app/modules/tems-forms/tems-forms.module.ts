import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatOptionModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatBadgeModule } from '@angular/material/badge';
import { MatMenuModule } from '@angular/material/menu';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { FormlyModule } from '@ngx-formly/core';
import { FormlyMaterialModule } from '@ngx-formly/material';
import { TranslateModule } from '@ngx-translate/core';

export const TEMS_FORMS_IMPORTS = [
  CommonModule,
  RouterModule,
  FormsModule,
  ReactiveFormsModule,
  TranslateModule,
  MatFormFieldModule,
  MatInputModule,
  MatAutocompleteModule,
  MatOptionModule,
  MatSelectModule,
  MatCheckboxModule,
  MatChipsModule,
  MatIconModule,
  MatTooltipModule,
  MatBadgeModule,
  MatMenuModule,
  MatTabsModule,
  MatProgressBarModule,
  FormlyMaterialModule,
  FormlyModule
];

