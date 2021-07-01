import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { GenericContainerComponent } from './generic-container.component';

@NgModule({
  declarations: [
    GenericContainerComponent
  ],
  imports: [
    CommonModule,
    MatIconModule,
    MatCardModule,
    MatMenuModule,
    MatButtonModule
  ],
  exports: [
    GenericContainerComponent
  ]
})
export class GenericContainerModule { }
