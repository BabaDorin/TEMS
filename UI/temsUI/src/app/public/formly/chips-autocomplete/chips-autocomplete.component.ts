import { IOption } from './../../../models/option.model';
import { COMMA, ENTER, SPACE } from '@angular/cdk/keycodes';
import { Component, ElementRef, Input, ViewChild, EventEmitter, Output, OnInit, forwardRef, SimpleChanges, SimpleChange } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatAutocompleteSelectedEvent, MatAutocomplete } from '@angular/material/autocomplete';
import { MatChipInputEvent } from '@angular/material/chips';
import { allowedNodeEnvironmentFlags } from 'process';
import { Observable, Subscription } from 'rxjs';
import { debounceTime, map, startWith, switchMap } from 'rxjs/operators';

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
  options: IOption[];
  selectable = true;
  removable = true;
  visible = true;
  value = [];
  cancelOnChange = true;
  ngOnInit() {
    console.log('already selected')
    console.log(this.alreadySelected);
    this.options = this.alreadySelected;
    this.listenToServer();
  }

  ngOnChanges(changes: { [propName: string]: SimpleChange }) {
    if(this.cancelOnChange){
      this.cancelOnChange = false;
      return;
    }


    if(changes['endPoint'] && changes['endPoint'].previousValue != changes['endPoint'].currentValue ) {
      this.options = [];
      this.value = [];
    }

    this.filteredOptions = [];
    this.listenToServer();
    this.options = (this.alreadySelected == undefined) ? [] : this.alreadySelected;
  }

  listenToServer(){
    this.subscription.unsubscribe();

    if(this.autocompleteOptions != undefined)
      this.getFromAutocompleteOptions();
    else
      this.getFromServer();
  }

  getFromAutocompleteOptions(){
    this.subscription = this.formCtrl.valueChanges
    .subscribe(op => {
      this.filteredOptions = this.autocompleteOptions.filter(q => q.label.toLowerCase().includes(op.toString().toLowerCase()) ?? []);
    });
  }

  getFromServer(){
    this.subscription = this.formCtrl.valueChanges
    .pipe(
      switchMap((op) => {
        return (this.endPointParameter == undefined) 
        ? this.endPoint.getAllAutocompleteOptions(op)
        : this.endPoint.getAllAutocompleteOptions(op, this.endPointParameter)
      }))
      .subscribe(data => {
        console.log(data);
        this.filteredOptions = (data as IOption[]);

      if(this.options != undefined)
        this.filteredOptions = this.filteredOptions
          .filter((el) => !this.options.includes(el));
      }
    );
  }

  // When the option has been typed
  add(event: MatChipInputEvent): void {
    const value = event.value;
    let typedOption = { value: undefined, label: value };

    // If accepting only values from dropdown
    if (this.onlyValuesFromAutocomplete == true) {
      typedOption = this.filteredOptions.find(q => q.label.toLowerCase() == value.toLowerCase());
    }

    if (typedOption != undefined) {
      if(this.isValueAlreadySelected(typedOption))
        return;

      this.maxOptionsSelectedValidation();
      this.options.push(typedOption);
      this.dataCollected.emit(this.options);
      this.formCtrl.setValue('');
      this.optionInput.nativeElement.value = '';
    }

    this.onChange(this.options);
  }

  // When the option has been chosen
  selected(event: MatAutocompleteSelectedEvent): void {
    if(this.isValueAlreadySelected(event.option.value))
      return;

    this.maxOptionsSelectedValidation();
    this.options.push(event.option.value);
    console.log(this.options);
    this.onChange(this.options);
    this.dataCollected.emit(this.options);

    this.formCtrl.setValue('');
    this.optionInput.nativeElement.value = '';
  }

  isValueAlreadySelected(value: IOption): boolean{
    if(this.options == undefined || this.options.length == 0)
      return false;

    return this.options.findIndex(
      (el) => 
      el.value == value.value
      && el.label == value.label
      && el.additional == value.additional) > -1
  }

  remove(op): void {
    const index = this.options.indexOf(op);
    if (index >= 0) {
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
    
    if (this.maxOptionsSelected == this.options.length)
      this.options.pop();
  }

  onChange: any = () => { };
  onTouched: any = () => { };

  writeValue(options: IOption[]): void {
    this.value = options;
    if(Array.isArray(options)){
      this.options = options;
    }
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
