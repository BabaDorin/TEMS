import { IOption } from './../../../models/option.model';
import { COMMA, ENTER, SPACE } from '@angular/cdk/keycodes';
import { Component, ElementRef, Input, ViewChild, EventEmitter, Output, OnInit, forwardRef } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatAutocompleteSelectedEvent, MatAutocomplete } from '@angular/material/autocomplete';
import { MatChipInputEvent } from '@angular/material/chips';
import { allowedNodeEnvironmentFlags } from 'process';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

@Component({
  selector: 'app-chips-autocomplete',
  templateUrl: './chips-autocomplete.component.html',
  styleUrls: ['./chips-autocomplete.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ChipsAutocompleteComponent),
      multi: true
    }
  ]
})
export class ChipsAutocompleteComponent implements OnInit, ControlValueAccessor {

  // ISSUES: 1) DISPLAY AUTOCOMPLETE LIST ON CLICK


  @Input() alreadySelected: IOption[] = [];
  @Input() label: string;
  @Input() disabled: boolean = false;
  @Input() maxOptionsSelected: number;
  @Input() placeholder: string = 'New Option...';
  @Input() onlyValuesFromAutocomplete: boolean = true;
  @Input() availableOptions: IOption[] = [];

  // returns results to parent components
  @Output() dataCollected = new EventEmitter;
  @Output() dataRemoved = new EventEmitter;
  // user has already typed - to update the list of options (if needed)
  @Output() Typing = new EventEmitter;

  @ViewChild('optionInput') optionInput: ElementRef<HTMLInputElement>;
  @ViewChild('auto') matAutocomplete: MatAutocomplete;

  visible = true;
  selectable = true;
  removable = true;
  separatorKeysCodes: number[] = [ENTER, COMMA];
  formCtrl = new FormControl();
  filteredOptions: Observable<IOption[]>;
  options: IOption[]; // Selected values
  value = []; // Value accessor

  ngOnInit() {
    this.options = this.alreadySelected;
    if(this.availableOptions == undefined) this.availableOptions = [];
  }

  ngOnChanges() {
    // When parent component updates some input parameters
    this.filteredOptions = this.formCtrl.valueChanges.pipe(
      startWith(null),
      map((op) => op ? this._filter(op) : this.availableOptions.slice()));

    this.options = (this.alreadySelected == undefined) ? [] : this.alreadySelected;
  }

  constructor() {
    this.filteredOptions = this.formCtrl.valueChanges.pipe(
      startWith(null),
      map((op) => op ? this._filter(op) : this.availableOptions.slice()));
  }

  // When the option has been typed
  add(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    let typedOption = { value: undefined, label: value };

    // If accepting only values from dropdown
    if (this.onlyValuesFromAutocomplete == true) {
      typedOption = this.availableOptions.find(q => q.label.toLowerCase() == value.toLowerCase());
    }

    if (typedOption != undefined) {
      this.maxOptionsSelectedValidation();

      this.options.push(typedOption);

      // Removing the typed option from availableOptions (if it exists)
      if (this.availableOptions.find(q => q.label.toLowerCase() == value.toLowerCase()) != undefined) {
        this.availableOptions.splice(this.availableOptions.indexOf(typedOption), 1)
      }

      input.value = '';
      this.dataCollected.emit(this.options);
    }

    this.onChange(this.options);
    this.formCtrl.setValue(null);
  }

  // When the option has been chosen
  selected(event: MatAutocompleteSelectedEvent): void {
    
    this.maxOptionsSelectedValidation();
    this.options.push(event.option.value);

    let index = this.availableOptions.indexOf(event.option.value);
    this.availableOptions.splice(index, 1);
    this.optionInput.nativeElement.value = '';
    this.formCtrl.setValue(null);
    this.onChange(this.options);
    this.dataCollected.emit(this.options);
  }

  remove(op): void {
    const index = this.options.indexOf(op);
    if (index >= 0) {
      // Pushing option back to available option if it belongs there
      if (op.value != undefined) {
        this.availableOptions.push(op);
      }

      this.options.splice(index, 1);
      this.formCtrl.updateValueAndValidity();
    }
    this.onChange(this.options);
    this.dataRemoved.emit(op);
  }


  // if there is a maxOptionsSelected specified
  maxOptionsSelectedValidation() {
    if(this.maxOptionsSelected == undefined)
      return;
    
    if (this.maxOptionsSelected == this.options.length){
      // If value belongs to available option, then it is inserted back there
      // Otherwise, it is just pops out from options list.
      let poppedOption = this.options.pop();
      
      if(poppedOption.value != undefined)
        this.availableOptions.push(poppedOption);
    }
  }

  private _filter(op): IOption[] {
    const filterValue = (typeof (op) == "string") ? op.toLowerCase() : op.label.toLowerCase();
    console.log('filter value: ' + filterValue);
    return this.availableOptions.filter(option => option.label.toLowerCase().indexOf(filterValue) === 0);
  }





  onChange: any = () => { };
  onTouched: any = () => { };

  writeValue(options: IOption[]): void {
    this.value = options;
  }
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
