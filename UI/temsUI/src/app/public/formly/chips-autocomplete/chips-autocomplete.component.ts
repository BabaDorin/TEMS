import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component, ElementRef, EventEmitter, forwardRef, Input, OnChanges, OnInit, Output, SimpleChange, ViewChild } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatAutocomplete, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatChipInputEvent } from '@angular/material/chips';
import { Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { isNullOrEmpty } from 'src/app/helpers/validators/validations';
import { IOption } from './../../../models/option.model';

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
export class ChipsAutocompleteComponent implements OnInit, ControlValueAccessor, OnChanges {

  // BEFREE: 1) DISPLAY AUTOCOMPLETE LIST ON CLICK
  @Input() alreadySelected: IOption[] = [];
  @Input() label: string;
  @Input() disabled: boolean = false;
  @Input() maxOptionsSelected: number;
  @Input() placeholder: string = 'New Option...';
  @Input() onlyValuesFromAutocomplete: boolean = true;
  @Input() availableOptions: IOption[] = [];
  @Input() endPoint;
  @Input() endPointParameter;
  @Input() autocompleteOptions: IOption[];

  @Output() dataCollected = new EventEmitter;
  @Output() dataRemoved = new EventEmitter;
  @Output() Typing = new EventEmitter;

  @ViewChild('optionInput') optionInput: ElementRef<HTMLInputElement>;
  @ViewChild('auto') matAutocomplete: MatAutocomplete;

  separatorKeysCodes: number[] = [ENTER, COMMA];
  subscription = new Subscription();
  formCtrl = new FormControl();
  filteredOptions: IOption[];
  selectedOptions: IOption[] = [];
  selectable = true;
  removable = true;
  visible = true;
  value = [];
  cancelOnChange = true;

  // When we type say 'bill' in input, then we select the 'Bill Gates' option from the dropdown, when we press
  // enter on the dropdown, both add and selected eventhandlers will be actioned.
  // In this case, we're interested only in the value from dropdown, this is a flag that helps us to achieve the desired
  // behavior
  enterFiredOnDropdownOption: boolean = false;

  set options(value) {
    this.selectedOptions = value;
  }

  get options() {
    return this.selectedOptions
  }

  constructor() {
  }

  ngOnInit() {
    this.selectedOptions = this.alreadySelected ?? [];
    this.onChange(this.selectedOptions);
    this.listenToServer();
  }

  ngOnChanges(changes: { [propName: string]: SimpleChange }) {
    if (this.cancelOnChange) {
      this.cancelOnChange = false;
      return;
    }

    if (changes['endPoint'] && changes['endPoint'].previousValue != changes['endPoint'].currentValue) {
      this.selectedOptions = [];
      this.value = [];
      this.onChange(this.selectedOptions);
    }

    if (changes['autocompleteOptions'] && changes['autocompleteOptions'].previousValue != changes['autocompleteOptions'].currentValue) {
      if(this.autocompleteOptions != undefined)
        this.getFromAutocompleteOptions();
    }

    this.filteredOptions = [];
    this.listenToServer();
    this.options = (this.alreadySelected == undefined) ? [] : this.alreadySelected;
    this.onChange(this.selectedOptions);
  }

  listenToServer() {
    this.subscription.unsubscribe();
    if (this.autocompleteOptions != undefined)
      this.getFromAutocompleteOptions();
    else
      this.getFromServer();
  }

  getFromAutocompleteOptions() {
    this.subscription = this.formCtrl.valueChanges
      .subscribe(op => {
        this.filteredOptions = this.autocompleteOptions.filter(q => q.label.toLowerCase().includes(op?.toString().toLowerCase()) ?? []);
      });
  }

  getFromServer() {
    this.subscription = this.formCtrl.valueChanges
      .pipe(
        switchMap((op) => {
          if(op == undefined) op = '';
          return (this.endPointParameter == undefined)
            ? this.endPoint.getAllAutocompleteOptions(op)
            : this.endPoint.getAllAutocompleteOptions(op, this.endPointParameter)
        }))
      .subscribe(data => {
        this.filteredOptions = (data as IOption[]);

        if (this.selectedOptions != undefined)
          this.filteredOptions = this.filteredOptions
            .filter((el) => !this.selectedOptions.includes(el));
      }
      );
  }

  optionActivated(event){
    console.log('---------------');
    this.enterFiredOnDropdownOption = true;
  }
  
  // When the option has been typed
  add(event: MatChipInputEvent): void {
    const value = event.value;
    if(isNullOrEmpty(value))
      return;

    let typedOption = { value: value, label: value };

    // If accepting only values from dropdown
    if (this.onlyValuesFromAutocomplete == true) {
      typedOption = this.filteredOptions.find(q => q.label.toLowerCase() == value.toLowerCase());
    }

    if (typedOption != undefined) {
      console.log('here 1  with ' + value);
      if (this.isValueAlreadySelected(typedOption))
        return;

      console.log('here 2  with ' + value);

      this.maxOptionsSelectedValidation();
      this.selectedOptions.push(typedOption);
      console.log('here 3  with ' + value);

      this.dataCollected.emit(this.selectedOptions);
      this.formCtrl.setValue('');
      this.optionInput.nativeElement.value = '';
    }

    this.onChange(this.selectedOptions);
  }

  // When the option has been chosen
  selected(event: MatAutocompleteSelectedEvent): void {
    console.log('selected event');
    console.log(event);
    
    this.enterFiredOnDropdownOption = true;
    console.log('selected: ' + this.enterFiredOnDropdownOption);

    if (this.isValueAlreadySelected(event.option.value))
      return;

    this.maxOptionsSelectedValidation();
    this.selectedOptions.push(event.option.value);
    this.onChange(this.selectedOptions);
    this.dataCollected.emit(this.selectedOptions);

    this.formCtrl.setValue('');
    this.optionInput.nativeElement.value = '';

    this.enterFiredOnDropdownOption = false;
  }

  isValueAlreadySelected(value: IOption): boolean {
    if (this.selectedOptions == undefined || this.selectedOptions.length == 0)
      return false;

    return this.selectedOptions.findIndex(
      (el) => el.value == value.value) > -1
  }

  remove(op): void {
    const index = this.selectedOptions.indexOf(op);
    if (index >= 0) {
      this.selectedOptions.splice(index, 1);
      this.formCtrl.updateValueAndValidity();
    }
    this.onChange(this.selectedOptions);
    this.dataRemoved.emit(op);
  }

  // if there is a maxOptionsSelected specified
  maxOptionsSelectedValidation() {
    if (this.maxOptionsSelected == undefined)
      return;

    if (this.maxOptionsSelected == this.selectedOptions.length)
      this.selectedOptions.pop();
  }

  onChange: any = () => { };
  onTouched: any = () => { };

  writeValue(options: IOption[]): void {
    this.value = options;
    if (Array.isArray(options)) {
      this.selectedOptions = options;
    };
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
