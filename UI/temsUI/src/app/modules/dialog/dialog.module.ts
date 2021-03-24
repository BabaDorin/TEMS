import { AppModule } from './../../app.module';
import { DialogService } from './../../services/dialog-service/dialog.service';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MatDialogModule,
    TemsFormsModule,
  ],
  providers:[
    DialogService
  ]
})
export class DialogModule { }
