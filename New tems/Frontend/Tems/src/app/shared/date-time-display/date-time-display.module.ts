import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { DateTimeDisplayComponent } from './date-time-display.component';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    DateTimeDisplayComponent
  ],
  exports:[
    DateTimeDisplayComponent
  ]
})
export class DateTimeDisplayModule { }
