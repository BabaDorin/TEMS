import { isNullOrEmpty } from 'src/app/helpers/validators/validations';
import { TranslateService } from '@ngx-translate/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Component, Input, OnInit, Output, EventEmitter, forwardRef } from '@angular/core';

@Component({
  selector: 'app-include-equipment-labels',
  templateUrl: './include-equipment-tags.component.html',
  styleUrls: ['./include-equipment-tags.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => IncludeEquipmentLabelsComponent),
      multi: true
    }
  ]
})
export class IncludeEquipmentLabelsComponent implements OnInit, ControlValueAccessor{

  public value: string[];

  getSelectedLabels(): string[]{
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
  @Input() defaultValue: string[] = []; // When nothin is selected, default value will be applied
  
  // Tag options (By default all of them are included)
  @Input() tagOptions = ['Equipment', 'Component', 'Part'];

  @Output() valueChanged = new EventEmitter();

  onChange;
  onTouch;

  constructor(public translate: TranslateService) { }
  
  ngOnInit(): void {
    this.setValue(false);
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

  setValue(emitNotification: boolean = true){
    this.value = this.getSelectedLabels();
    if(isNullOrEmpty(this.value)){
      this.value = this.defaultValue;
      
      if(this.defaultValue.includes('Equipment'))
        this.includeEquipment = true;

      if(this.defaultValue.includes('Component'))
        this.includeComponents = true;

      if(this.defaultValue.includes('Part'))
        this.includeParts = true;
    }
 
    // Quick workaround for registerOnChange being called after the first initialization
    // BEFREE: Find a more ingenious solution.
    if(this.onChange == undefined)
    {
      // let's wait 50 miliseconds (registed on change might not been called yet)
      setTimeout(() => {
        if(emitNotification)
          this.valueChanged.emit(this.value);
        // If even here onChange is not defined, it means that this component is not used with as a formControl.
        if(onchange == undefined)
          return;

        this.onChange(this.value);
      }, 50);
    }
    else{
      this.onChange(this.value);

      if(emitNotification)
          this.valueChanged.emit(this.value);
    }
  }
}
