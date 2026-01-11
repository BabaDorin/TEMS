import { Component, Input, Output, EventEmitter, forwardRef, OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { DropdownManagerService } from './dropdown-manager.service';
import { Subscription } from 'rxjs';

export interface SelectOption {
  value: string;
  label: string;
}

let instanceCounter = 0;

@Component({
  selector: 'app-custom-select',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './custom-select.component.html',
  styleUrls: ['./custom-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CustomSelectComponent),
      multi: true
    }
  ]
})
export class CustomSelectComponent implements ControlValueAccessor, OnInit, OnDestroy {
  @Input() options: SelectOption[] = [];
  @Input() placeholder: string = 'Select...';
  @Input() searchable: boolean = true;
  @Input() disabled: boolean = false;
  @Input() allowEmpty: boolean = true;
  @Input() emptyLabel: string = 'None';
  @Input() mode: 'single' | 'multiple' = 'single';

  instanceId: number;
  isOpen = false;
  searchText = '';
  private _selectedValue: string = '';
  selectedValues: string[] = [];
  private subscription: Subscription = new Subscription();

  get selectedValue(): string {
    return this._selectedValue;
  }

  set selectedValue(val: string) {
    this._selectedValue = val;
    this.cdr.markForCheck();
  }

  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  constructor(
    private cdr: ChangeDetectorRef,
    private dropdownManager: DropdownManagerService
  ) {
    this.instanceId = ++instanceCounter;
  }

  ngOnInit() {
    this.subscription.add(
      this.dropdownManager.closeAll$.subscribe(openedInstanceId => {
        if (openedInstanceId !== this.instanceId && this.isOpen) {
          this.isOpen = false;
          this.searchText = '';
          this.cdr.markForCheck();
        }
      })
    );
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  get filteredOptions(): SelectOption[] {
    if (!this.searchText) {
    if (!this.isOpen) {
      this.dropdownManager.notifyOpen(this.instanceId);
    }
      return this.options;
    }
    return this.options.filter(opt => 
      opt.label.toLowerCase().includes(this.searchText.toLowerCase())
    );
  }

  get selectedLabel(): string {
    if (this._selectedValue === null || this._selectedValue === undefined || this._selectedValue === '') {
      return this.placeholder;
    }
    const option = this.options.find(opt => opt.value === this._selectedValue);
    return option ? option.label : this.placeholder;
  }

  toggleDropdown() {
    if (this.disabled) return;
    this.isOpen = !this.isOpen;
    if (!this.isOpen) {
      this.searchText = '';
    }
    this.cdr.markForCheck();
  }

  selectOption(value: string): void {
    if (this.mode === 'single') {
      this._selectedValue = value;
      this.onChange(value);
      this.onTouched();
      this.isOpen = false;
      this.searchText = '';
      this.cdr.markForCheck();
    }
  }

  isSelected(value: string): boolean {
    return this._selectedValue === value;
  }

  trackByValue(index: number, option: SelectOption): string {
    return option.value;
  }

  writeValue(value: string | null): void {
    if (value === null || value === undefined) {
      this._selectedValue = '';
    } else {
      this._selectedValue = value;
    }
    this.cdr.markForCheck();
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
