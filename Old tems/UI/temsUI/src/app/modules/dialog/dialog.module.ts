import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';
import { DialogService } from '../../services/dialog.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    MatDialogModule,
    ReactiveFormsModule
  ],
  providers:[
    DialogService,
  ]
})
export class DialogModule { }
