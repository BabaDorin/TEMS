import { Component, Input, forwardRef, OnInit, OnDestroy, OnChanges, SimpleChanges, ChangeDetectionStrategy, ChangeDetectorRef, ElementRef, HostListener, ViewChild, NgZone } from '@angular/core';
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
export class CustomSelectComponent implements ControlValueAccessor, OnInit, OnDestroy, OnChanges {
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

  dropdownTop = 0;
  dropdownLeft = 0;
  dropdownWidth = 0;
  dropdownDirection: 'down' | 'up' = 'down';

  get selectedValue(): string {
    return this._selectedValue;
  }

  set selectedValue(val: string) {
    this._selectedValue = val;
    this.cdr.markForCheck();
  }

  private onChange: (value: string | string[]) => void = () => {};
  private onTouched: () => void = () => {};

  constructor(
    private cdr: ChangeDetectorRef,
    private dropdownManager: DropdownManagerService,
    private elementRef: ElementRef,
    private ngZone: NgZone
  ) {
    this.instanceId = ++instanceCounter;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (this.isOpen && !this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen = false;
      this.searchText = '';
      this.cdr.markForCheck();
    }
  }

  @HostListener('window:resize')
  @HostListener('window:scroll', ['$event'])
  onWindowChange() {
    if (this.isOpen) {
      this.updateDropdownPosition();
    }
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

    this.ngZone.runOutsideAngular(() => {
      document.addEventListener('scroll', this.onScrollCapture, true);
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['options']) {
      this.cdr.detectChanges();
    }
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    document.removeEventListener('scroll', this.onScrollCapture, true);
  }

  private onScrollCapture = () => {
    if (this.isOpen) {
      this.ngZone.run(() => {
        this.updateDropdownPosition();
      });
    }
  };

  get filteredOptions(): SelectOption[] {
    if (!this.searchText) {
      return this.options;
    }
    return this.options.filter(opt => 
      opt.label.toLowerCase().includes(this.searchText.toLowerCase())
    );
  }

  get selectedLabel(): string {
    if (this.mode === 'multiple') {
      if (this.selectedValues.length === 0) {
        return this.placeholder;
      }
      if (this.selectedValues.length === 1) {
        const option = this.options.find(opt => opt.value === this.selectedValues[0]);
        return option ? option.label : this.placeholder;
      }
      return `${this.selectedValues.length} selected`;
    }
    
    if (this._selectedValue === null || this._selectedValue === undefined || this._selectedValue === '') {
      return this.placeholder;
    }
    const option = this.options.find(opt => opt.value === this._selectedValue);
    return option ? option.label : this.placeholder;
  }

  updateDropdownPosition() {
    const trigger = this.elementRef.nativeElement.querySelector('.select-trigger');
    if (!trigger) return;

    const rect = trigger.getBoundingClientRect();
    const viewportHeight = window.innerHeight;
    const dropdownMaxHeight = 280;
    const spaceBelow = viewportHeight - rect.bottom;
    const spaceAbove = rect.top;

    this.dropdownWidth = rect.width;
    this.dropdownLeft = rect.left;

    if (spaceBelow >= dropdownMaxHeight || spaceBelow >= spaceAbove) {
      this.dropdownDirection = 'down';
      this.dropdownTop = rect.bottom + 4;
    } else {
      this.dropdownDirection = 'up';
      this.dropdownTop = rect.top - dropdownMaxHeight - 4;
    }

    this.cdr.markForCheck();
  }

  toggleDropdown() {
    if (this.disabled) return;
    this.isOpen = !this.isOpen;
    if (this.isOpen) {
      this.dropdownManager.notifyOpen(this.instanceId);
      this.updateDropdownPosition();
    }
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
    } else {
      const index = this.selectedValues.indexOf(value);
      if (index > -1) {
        this.selectedValues = this.selectedValues.filter(v => v !== value);
      } else {
        this.selectedValues = [...this.selectedValues, value];
      }
      this.onChange(this.selectedValues);
      this.onTouched();
      this.cdr.markForCheck();
    }
  }

  isSelected(value: string): boolean {
    if (this.mode === 'multiple') {
      return this.selectedValues.includes(value);
    }
    return this._selectedValue === value;
  }

  trackByValue(index: number, option: SelectOption): string {
    return option.value;
  }

  getOptionLabel(value: string): string {
    const option = this.options.find(opt => opt.value === value);
    return option ? option.label : value;
  }

  writeValue(value: string | string[] | null): void {
    if (this.mode === 'multiple') {
      if (Array.isArray(value)) {
        this.selectedValues = [...value];
      } else {
        this.selectedValues = [];
      }
    } else {
      if (value === null || value === undefined) {
        this._selectedValue = '';
      } else {
        this._selectedValue = Array.isArray(value) ? '' : value;
      }
    }
    this.cdr.detectChanges();
  }

  registerOnChange(fn: (value: string | string[]) => void): void {
    this.onChange = fn as any;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
