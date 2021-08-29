import { IIncludeEquipmentTypes } from './../../models/equipment/include-equipment-tags.model';
import { TranslateService } from '@ngx-translate/core';
import { ControlValueAccessor } from '@angular/forms';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-include-equipment-tags',
  templateUrl: './include-equipment-tags.component.html',
  styleUrls: ['./include-equipment-tags.component.scss']
})
export class IncludeEquipmentTagsComponent implements OnInit, ControlValueAccessor{

  value: IIncludeEquipmentTypes;
  
  @Input() includeEquipment: boolean = true;
  @Input() includeParts: boolean = false;
  @Input() includeComponents: boolean = false;

  @Output() valueChanged = new EventEmitter();

  onChange;
  onTouch;

  constructor(private translate: TranslateService) { }
  
  ngOnInit(): void {
    this.setValue();
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouch = fn;
  }

  writeValue(obj: any): void {
    if(obj == null || obj == undefined)
      return;

    this.value = obj;
  }

  setValue(){
    this.value = {
      includeEquipment: this.includeComponents,
      includeParts: this.includeParts,
      includeComponents: this.includeComponents
    };
 
    // Quick workaround for registerOnChange being called after the first initialization
    // BEFREE: Find a more ingenious solution.
    if(this.onChange == undefined)
    {
      setTimeout(() => {
        this.onChange(this.value);
        this.valueChanged.emit(this.value);
      }, 50);
    }
    else{
      this.onChange(this.value);
      this.valueChanged.emit(this.value);
    }
  }
}
