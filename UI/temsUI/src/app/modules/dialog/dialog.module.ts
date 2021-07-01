import { DialogService } from '../../services/dialog.service';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MatDialogModule,
    // TemsFormsModule,
  ],
  providers:[
    DialogService,
    TranslateModule
  ]
})
export class DialogModule { }
