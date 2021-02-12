import {COMMA, ENTER} from '@angular/cdk/keycodes';
import { Component, ElementRef, Input, ViewChild, EventEmitter, Output } from '@angular/core';
import {FormControl} from '@angular/forms';
import {MatAutocompleteSelectedEvent, MatAutocomplete} from '@angular/material/autocomplete';
import {MatChipInputEvent} from '@angular/material/chips';
import {Observable} from 'rxjs';
import {map, startWith} from 'rxjs/operators';

@Component({
  selector: 'app-chips-autocomplete',
  templateUrl: './chips-autocomplete.component.html',
  styleUrls: ['./chips-autocomplete.component.scss']
})
export class ChipsAutocompleteComponent {

  // List of selected options
  @Input() alreadySelected;
  // List of available for selection options
  @Input() availableOptions;

  // returns results to parent components
  @Output() dataCollected = new EventEmitter;
  // Nofities parent that user has typed something, and returns what the
  // user has already typed - to update the list of options (if needed)
  @Output() Typing = new EventEmitter;
  
  visible = true;
  selectable = true;
  removable = true;
  separatorKeysCodes: number[] = [ENTER, COMMA];
  formCtrl = new FormControl();
  filteredOptions: Observable<string[]>;
  options: string[] = []
  allOptions: string[] = ['Apple', 'Lemon', 'Lime', 'Orange', 'Strawberry'];

  @ViewChild('optionInput') optionInput: ElementRef<HTMLInputElement>;
  @ViewChild('auto') matAutocomplete: MatAutocomplete;

  constructor() {
    this.filteredOptions = this.formCtrl.valueChanges.pipe(
        startWith(null),
        map((op: string | null) => op ? this._filter(op) : this.allOptions.slice()));
  }

  add(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    // Add our fruit
    if ((value || '').trim()) {
      this.options.push(value.trim());
    }

    // Reset the input value
    if (input) {
      input.value = '';
    }

    this.formCtrl.setValue(null);
  }

  remove(fruit: string): void {
    const index = this.options.indexOf(fruit);

    if (index >= 0) {
      this.options.splice(index, 1);
    }
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    this.options.push(event.option.viewValue);
    this.optionInput.nativeElement.value = '';
    this.formCtrl.setValue(null);
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();

    return this.allOptions.filter(op => op.toLowerCase().indexOf(filterValue) === 0);
  }
}
