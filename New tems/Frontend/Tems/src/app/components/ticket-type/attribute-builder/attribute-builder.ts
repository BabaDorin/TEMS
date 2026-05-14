import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AttributeDefinition } from '../../../models/ticket/ticket-type.model';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

@Component({
  selector: 'app-attribute-builder',
  imports: [CommonModule, FormsModule, CustomSelectComponent],
  templateUrl: './attribute-builder.html',
  styleUrl: './attribute-builder.scss',
})
export class AttributeBuilder {
  @Input() attributes: AttributeDefinition[] = [];
  @Output() attributesChange = new EventEmitter<AttributeDefinition[]>();
  maxAttributes = 50;

  dataTypes = [
    { value: 'STRING', label: 'Text' },
    { value: 'BOOL', label: 'Boolean' },
    { value: 'DROPDOWN', label: 'Dropdown' }
  ];

  get dataTypeOptions(): SelectOption[] {
    return this.dataTypes.map((type) => ({ value: type.value, label: type.label }));
  }

  addAttribute(): void {
    if (this.attributes.length >= this.maxAttributes) {
      return;
    }

    const newAttribute: AttributeDefinition = {
      key: '',
      label: '',
      dataType: 'STRING',
      isRequired: false,
      options: []
    };

    this.attributes.push(newAttribute);
    this.attributesChange.emit(this.attributes);
  }

  removeAttribute(index: number): void {
    this.attributes.splice(index, 1);
    this.attributesChange.emit(this.attributes);
  }

  addOption(attribute: AttributeDefinition): void {
    if (!attribute.options) {
      attribute.options = [];
    }
    attribute.options.push('');
    this.attributesChange.emit(this.attributes);
  }

  removeOption(attribute: AttributeDefinition, optionIndex: number): void {
    if (attribute.options) {
      attribute.options.splice(optionIndex, 1);
      this.attributesChange.emit(this.attributes);
    }
  }

  trackByIndex(index: number): number {
    return index;
  }

  onDataTypeChange(attribute: AttributeDefinition): void {
    if (attribute.dataType === 'DROPDOWN' && !attribute.options) {
      attribute.options = [];
    }
    this.attributesChange.emit(this.attributes);
  }

  onAttributeChange(): void {
    this.attributesChange.emit(this.attributes);
  }
}
