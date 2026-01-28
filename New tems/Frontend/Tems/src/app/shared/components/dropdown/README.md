# Dropdown Component Usage

## Import

```typescript
import { DropdownComponent, DropdownOption } from 'src/app/shared/components/dropdown/dropdown.component';

@Component({
  // ...
  imports: [DropdownComponent, ReactiveFormsModule, FormsModule]
})
```

## Basic Usage

### Single Select

```typescript
// In your component
export class MyComponent {
  selectedValue: string | null = null;
  
  options: DropdownOption[] = [
    { label: 'Option 1', value: '1' },
    { label: 'Option 2', value: '2' },
    { label: 'Option 3', value: '3' },
    { label: 'Disabled Option', value: '4', disabled: true }
  ];

  onSelectionChange(value: any) {
    console.log('Selected:', value);
  }
}
```

```html
<!-- In your template -->
<app-dropdown
  [options]="options"
  [(ngModel)]="selectedValue"
  placeholder="Select an option"
  (selectionChange)="onSelectionChange($event)"
></app-dropdown>
```

### Multi Select

```typescript
// In your component
export class MyComponent {
  selectedValues: string[] = [];
  
  options: DropdownOption[] = [
    { label: 'Type A', value: 'type-a' },
    { label: 'Type B', value: 'type-b' },
    { label: 'Type C', value: 'type-c' }
  ];
}
```

```html
<!-- In your template -->
<app-dropdown
  [options]="options"
  [(ngModel)]="selectedValues"
  [multiple]="true"
  placeholder="Select types"
></app-dropdown>
```

### With Reactive Forms

```typescript
// In your component
export class MyComponent {
  form = this.fb.group({
    category: [''],
    tags: [[]]
  });

  categoryOptions: DropdownOption[] = [
    { label: 'Electronics', value: 'electronics' },
    { label: 'Furniture', value: 'furniture' }
  ];

  tagOptions: DropdownOption[] = [
    { label: 'New', value: 'new' },
    { label: 'Sale', value: 'sale' },
    { label: 'Featured', value: 'featured' }
  ];
}
```

```html
<!-- In your template -->
<form [formGroup]="form">
  <!-- Single select -->
  <app-dropdown
    [options]="categoryOptions"
    formControlName="category"
    placeholder="Select category"
  ></app-dropdown>

  <!-- Multi select -->
  <app-dropdown
    [options]="tagOptions"
    formControlName="tags"
    [multiple]="true"
    placeholder="Select tags"
  ></app-dropdown>
</form>
```

### With Search

```html
<app-dropdown
  [options]="options"
  [(ngModel)]="selectedValue"
  [searchable]="true"
  placeholder="Search and select"
></app-dropdown>
```

## API

### Inputs

| Input | Type | Default | Description |
|-------|------|---------|-------------|
| `options` | `DropdownOption[]` | `[]` | Array of options to display |
| `placeholder` | `string` | `'Select...'` | Placeholder text when nothing is selected |
| `multiple` | `boolean` | `false` | Enable multi-select mode |
| `disabled` | `boolean` | `false` | Disable the dropdown |
| `searchable` | `boolean` | `false` | Enable search functionality |

### Outputs

| Output | Type | Description |
|--------|------|-------------|
| `selectionChange` | `EventEmitter<any>` | Emits when selection changes |

### DropdownOption Interface

```typescript
interface DropdownOption {
  label: string;      // Display text
  value: any;         // Option value
  disabled?: boolean; // Optional: disable this option
}
```

## Features

- ✅ Single and multi-select modes
- ✅ Selected values displayed as chips
- ✅ Smooth animated arrow rotation
- ✅ iOS-style design
- ✅ Reactive Forms compatible (ControlValueAccessor)
- ✅ Template-driven forms compatible (ngModel)
- ✅ Individual chip removal
- ✅ Clear all button (multi-select mode)
- ✅ Optional search functionality
- ✅ Disabled state support
- ✅ Click outside to close
- ✅ Keyboard accessible
- ✅ Smooth animations and transitions

## Styling

The component uses iOS-inspired styling:
- Clean borders instead of shadows
- Flat colors
- iOS blue (#007aff) for selections
- Smooth transitions
- Rounded corners (10px for container, 6px for chips)
