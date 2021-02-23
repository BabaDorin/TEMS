import { PropertyRenderComponent } from './../../public/property-render/property-render.component';
import { ChipsAutocompleteComponent } from './../../public/formly/chips-autocomplete/chips-autocomplete.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ImageCarouselComponent } from 'src/app/public/image-carousel/image-carousel.component';
import { AddLogComponent } from '../communication/add-log/add-log.component';



@NgModule({
  declarations: [
    ImageCarouselComponent,
    ChipsAutocompleteComponent,
    AddLogComponent,
    PropertyRenderComponent,
  ],
  imports: [
    CommonModule
  ],
  exports: [
    ImageCarouselComponent,
    ChipsAutocompleteComponent,
    AddLogComponent,
    PropertyRenderComponent,
  ]
})
export class EntitySharedModuleModule { }
