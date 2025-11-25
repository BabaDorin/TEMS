import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { LoadingPlaceholderComponent } from './../../tems-components/loading-placeholder/loading-placeholder.component';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    LoadingPlaceholderComponent
  ],
  exports: [
    LoadingPlaceholderComponent
  ]
})
export class LoadingplaceholderModule { }
