import { ConfirmService } from './../../confirm.service';
import { MatButtonModule } from '@angular/material/button';
import { ConfirmComponent } from './../../tems-components/confirm/confirm.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';



@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    ConfirmComponent,
    MatButtonModule,
  ],
  exports: [
    ConfirmComponent
  ],
  providers: [
    ConfirmService
  ]
})
export class ConfirmModule { }
