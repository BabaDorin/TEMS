import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AttributeDefinition } from '../../../models/ticket/ticket-type.model';

@Component({
  selector: 'app-attribute-builder',
  imports: [CommonModule, FormsModule],
  templateUrl: './attribute-builder.html',
  styleUrl: './attribute-builder.scss',
})
export class AttributeBuilder {
  @Input() attributes: AttributeDefinition[] = [];
  @Output() attributesChange = new EventEmitter<AttributeDefinition[]>();

  dataTypes = [
    { value: 'STRING', label: 'Text' },
    { value: 'BOOL', label: 'Boolean' },
    { value: 'DROPDOWN', label: 'Dropdown' }
  ];

  addAttribute(): void {
    if (this.attributes.length >= 50) {
      alert('Maximum 50 attributes allowed');
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
