import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { DialogService } from '../../services/dialog.service';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';


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
