import { Component, Input, Output, EventEmitter, forwardRef, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormsModule } from '@angular/forms';

export interface DropdownOption {
  label: string;
  value: any;
  disabled?: boolean;
}

@Component({
  selector: 'app-dropdown',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dropdown.component.html',
  styleUrls: ['./dropdown.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DropdownComponent),
      multi: true
    }
  ]
})
export class DropdownComponent implements ControlValueAccessor {
  @Input() set options(value: DropdownOption[]) {
    this._options = value;
    console.log('Dropdown received options:', value);
  }
  get options(): DropdownOption[] {
    return this._options;
  }
  private _options: DropdownOption[] = [];
  
  @Input() placeholder = 'Select...';
  @Input() multiple = false;
  @Input() disabled = false;
  @Input() searchable = false;
  @Output() selectionChange = new EventEmitter<any>();

  isOpen = false;
  searchText = '';
  selectedValues: any[] = [];

  private onChange: (value: any) => void = () => {};
  private onTouched: () => void = () => {};

  constructor(private elementRef: ElementRef) {}

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen = false;
    }
  }

  writeValue(value: any): void {
    if (value === null || value === undefined) {
      this.selectedValues = [];
    } else if (this.multiple) {
      this.selectedValues = Array.isArray(value) ? value : [value];
    } else {
      this.selectedValues = value ? [value] : [];
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  toggleDropdown(event: Event) {
    event.stopPropagation();
    if (!this.disabled) {
      this.isOpen = !this.isOpen;
      console.log('Dropdown toggled, isOpen:', this.isOpen, 'options count:', this.options.length);
      if (!this.isOpen) {
        this.onTouched();
      }
    }
  }

  isSelected(value: any): boolean {
    return this.selectedValues.includes(value);
  }

  toggleOption(option: DropdownOption, event: Event) {
    event.stopPropagation();
    
    if (option.disabled) return;

    if (this.multiple) {
      const index = this.selectedValues.indexOf(option.value);
      if (index > -1) {
        this.selectedValues = this.selectedValues.filter(v => v !== option.value);
      } else {
        this.selectedValues = [...this.selectedValues, option.value];
      }
      this.emitChange();
    } else {
      this.selectedValues = [option.value];
      this.isOpen = false;
      this.emitChange();
      this.onTouched();
    }
  }

  removeChip(value: any, event: Event) {
    event.stopPropagation();
    if (!this.disabled) {
      this.selectedValues = this.selectedValues.filter(v => v !== value);
      this.emitChange();
    }
  }

  clearAll(event: Event) {
    event.stopPropagation();
    if (!this.disabled) {
      this.selectedValues = [];
      this.emitChange();
      this.onTouched();
    }
  }

  private emitChange() {
    const value = this.multiple ? this.selectedValues : (this.selectedValues[0] || null);
    this.onChange(value);
    this.selectionChange.emit(value);
  }

  getSelectedOptions(): DropdownOption[] {
    return this.options.filter(opt => this.selectedValues.includes(opt.value));
  }

  getOptionLabel(value: any): string {
    return this.options.find(o => o.value === value)?.label || '';
  }

  getFilteredOptions(): DropdownOption[] {
    if (!this.searchText) {
      console.log('getFilteredOptions returning all options:', this.options);
      return this.options;
    }
    const search = this.searchText.toLowerCase();
    const filtered = this.options.filter(opt => 
      opt.label.toLowerCase().includes(search)
    );
    console.log('getFilteredOptions returning filtered:', filtered);
    return filtered;
  }

  get hasSelection(): boolean {
    return this.selectedValues.length > 0;
  }
}
