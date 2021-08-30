import { TranslateService } from '@ngx-translate/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Component, Input, OnInit, Output, EventEmitter, forwardRef } from '@angular/core';

@Component({
  selector: 'app-include-equipment-tags',
  templateUrl: './include-equipment-tags.component.html',
  styleUrls: ['./include-equipment-tags.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => IncludeEquipmentTagsComponent),
      multi: true
    }
  ]
})
export class IncludeEquipmentTagsComponent implements OnInit, ControlValueAccessor{

  public value: string[];

  getSelectedTags(): string[]{
    let result: string[] = [];

    if(this.value == undefined)
      return result;

    if(this.includeEquipment && this.tagOptions.includes('Equipment'))
      result.push('Equipment');

    if(this.includeComponents && this.tagOptions.includes('Component'))
      result.push('Component');
    
    if(this.includeParts && this.tagOptions.includes('Part'))
      result.push('Part');

    return result;
  }
  
  // Default values for tag flags
  @Input() includeEquipment: boolean = true;
  @Input() includeParts: boolean = false;
  @Input() includeComponents: boolean = false;
  
  // Tag options (By default all of them are included)
  @Input() tagOptions = ['Equipment', 'Component', 'Part'];

  @Output() valueChanged = new EventEmitter();

  onChange;
  onTouch;

  constructor(public translate: TranslateService) { }
  
  ngOnInit(): void {
    this.setValue();
  }

  registerOnChange(fn: any): void {
    console.log('resister on change called');
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
    console.log('inside setValue');
    this.value = this.getSelectedTags();
 
    this.valueChanged.emit(this.value);

    // Quick workaround for registerOnChange being called after the first initialization
    // BEFREE: Find a more ingenious solution.
    if(this.onChange == undefined)
    {
      // let's wait 50 miliseconds (registed on change might not been called yet)
      setTimeout(() => {
        // If even here onChange is not defined, it means that this component is not used with as a formControl.
        if(onchange == undefined)
          return;

        this.onChange(this.value);
      }, 50);
    }
    else{
      this.onChange(this.value);
    }
  }
}
