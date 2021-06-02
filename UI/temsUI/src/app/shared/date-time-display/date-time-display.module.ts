import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DateTimeDisplayComponent } from './date-time-display.component';

@NgModule({
  declarations: [
    DateTimeDisplayComponent
  ],
  imports: [
    CommonModule
  ],
  exports:[
    DateTimeDisplayComponent
  ]
})
export class DateTimeDisplayModule { }
