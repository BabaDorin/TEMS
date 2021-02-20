import { COMMA, ENTER, SPACE } from '@angular/cdk/keycodes';
import { Component, ElementRef, Input, ViewChild, EventEmitter, Output, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent, MatAutocomplete } from '@angular/material/autocomplete';
import { MatChipInputEvent } from '@angular/material/chips';
import { allowedNodeEnvironmentFlags } from 'process';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

@Component({
  selector: 'app-chips-autocomplete',
  templateUrl: './chips-autocomplete.component.html',
  styleUrls: ['./chips-autocomplete.component.scss']
})
export class ChipsAutocompleteComponent implements OnInit {

  // List of selected options
  @Input() alreadySelected;
  @Input() label;
  @Input() disabled: boolean;

  // List of available for selection options
  @Input() availableOptions;

  // returns results to parent components
  @Output() dataCollected = new EventEmitter;
  // Nofities parent that user has typed something, and returns what the
  // user has already typed - to update the list of options (if needed)
  @Output() Typing = new EventEmitter;

  @ViewChild('optionInput') optionInput: ElementRef<HTMLInputElement>;
  @ViewChild('auto') matAutocomplete: MatAutocomplete;

  visible = true;
  selectable = true;
  removable = true;
  separatorKeysCodes: number[] = [ENTER, COMMA, SPACE];
  formCtrl = new FormControl();
  filteredOptions: Observable<string[]>;
  options;

  ngOnInit() {
    if (this.disabled == undefined) this.disabled = false;
    if(this.availableOptions == undefined) this.availableOptions = [];
    this.options = this.alreadySelected;
  }

  ngOnChanges() {
    /**********THIS FUNCTION WILL TRIGGER WHEN PARENT COMPONENT UPDATES 'someInput'**************/
    //Write your code here
     this.filteredOptions = this.formCtrl.valueChanges.pipe(
      startWith(null),
      map((op) => op ? this._filter(op) : this.availableOptions.slice()));
  }  
  // ISSUES: 1) DISPLAY AUTOCOMPLETE LIST ON CLICK
  // 2) IF THERE ARE 4 ITEMS, SELECTING ONE AND THEN DELETING IT WILL RESULT IN 3 ITEMS IN AUTOCOMPLETE

  constructor() {
    this.filteredOptions = this.formCtrl.valueChanges.pipe(
      startWith(null),
      map((op) => op ? this._filter(op) : this.availableOptions.slice()));
  }

  add(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    // Add option
    let typedOption = this.availableOptions.find(q => q.value == value);
    if (typedOption != undefined) {
      this.options.push(typedOption);
      this.availableOptions.splice(this.availableOptions.indexOf(typedOption), 1)

      input.value = '';
    }

    this.formCtrl.setValue(null);
  }

  remove(op): void {
    console.log('remove called');
    const index = this.options.indexOf(op);

    if (index >= 0) {
      this.availableOptions.push(op);
      this.options.splice(index, 1);
      this.formCtrl.updateValueAndValidity();
    }
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    this.options.push(event.option.value);
    console.log(event.option.value);


    let index = this.availableOptions.indexOf(event.option.value);
    this.availableOptions.splice(index, 1);
    this.optionInput.nativeElement.value = '';
    this.formCtrl.setValue(null);
  }

  private _filter(op): string[] {

    const filterValue = (typeof (op) == "string") ? op.toLowerCase() : op.value.toLowerCase;
    return this.availableOptions.filter(op => op.value.toLowerCase().indexOf(filterValue) === 0);
  }
}
