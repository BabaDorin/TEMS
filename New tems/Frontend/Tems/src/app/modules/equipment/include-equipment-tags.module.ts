import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { IncludeEquipmentLabelsComponent } from './../../shared/include-equipment-tags/include-equipment-tags.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    IncludeEquipmentLabelsComponent,
    FormsModule, 
    MatSlideToggleModule,
    MatIconModule,
    MatTooltipModule,
    TranslateModule
  ],
  exports: [
    IncludeEquipmentLabelsComponent
  ]
})
export class IncludeEquipmentLabelsModule { }
