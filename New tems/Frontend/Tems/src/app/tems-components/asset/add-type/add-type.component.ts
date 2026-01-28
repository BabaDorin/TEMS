import { TranslateService } from '@ngx-translate/core';
import { Component, Inject, OnInit, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SnackService } from 'src/app/services/snack.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { AssetTypeService } from 'src/app/services/asset-type.service';
import { AssetPropertyService } from 'src/app/services/asset-property.service';
import { AssetType } from 'src/app/models/asset/asset-type.model';
import { AssetProperty } from 'src/app/models/asset/asset-property.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';


@Component({
  selector: 'app-add-type',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatIconModule,
    TranslateModule,
    ChipsAutocompleteComponent,
    CustomSelectComponent
  ],
  templateUrl: './add-type.component.html',
  styleUrls: ['./add-type.component.scss']
})

export class AddTypeComponent extends TEMSComponent implements OnInit {
  updateTypeId: string;
  formGroup: FormGroup;
  parentTypes: AssetType[] = [];
  availableProperties: AssetProperty[] = [];
  propertyMap: Record<string, AssetProperty> = {};
  submitAttempted = false;
  serverError = '';

  get propertyRows() {
    return this.formGroup.get('properties') as FormArray;
  }

  get propertyRowGroups(): FormGroup[] {
    return this.propertyRows.controls as FormGroup[];
  }

  get parentTypeOptions(): SelectOption[] {
    return this.parentTypes.map(t => ({ value: t.id, label: t.name }));
  }

  get propertyOptions(): SelectOption[] {
    return this.availableProperties
      .filter(p => (p as any)?.id || (p as any)?.propertyId)
      .map(p => ({
        value: (p as any).id ?? (p as any).propertyId,
        label: p.name
      }));
  }

  get allPropertiesSelected(): boolean {
    return this.propertyRowGroups.every(group => !!group.get('propertyId')?.value);
  }

  get canSubmit(): boolean {
    const nameValid = !!this.formGroup.get('name')?.value?.toString().trim();
    return nameValid && this.allPropertiesSelected;
  }

  get shouldShowClientError(): boolean {
    return !this.canSubmit && (this.submitAttempted || this.formGroup.touched);
  }

  trackByIndex(index: number): number {
    return index;
  }

  constructor(
    private assetTypeService: AssetTypeService,
    private assetPropertyService: AssetPropertyService,
    private snackService: SnackService,
    private fb: FormBuilder,
    public translate: TranslateService,
    @Optional() public dialogRef: MatDialogRef<AddTypeComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();
    this.updateTypeId = this.updateTypeId ?? this.dialogData?.updateTypeId;
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      parentTypeId: [''],
      properties: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.fetchTypes();
    this.fetchProperties();
    if (this.updateTypeId != undefined) {
      this.update();
    }
  }

  update() {
    this.subscriptions.push(
      this.assetTypeService.getById(this.updateTypeId)
        .subscribe(result => {
          this.formGroup.patchValue({
            name: result.name,
            description: result.description || '',
            parentTypeId: result.parentTypeId || ''
          });

          this.propertyRows.clear();
          (result.properties || [])
            .sort((a, b) => (a.displayOrder ?? 0) - (b.displayOrder ?? 0))
            .forEach((p) => {
              this.propertyRows.push(this.fb.group({
                propertyId: [p.propertyId, Validators.required],
                isRequired: [p.isRequired]
              }));
          });

          if (this.propertyRows.length === 0) {
            this.addPropertyRow();
          }
        })
    );
  }

  addPropertyRow() {
    this.propertyRows.push(this.fb.group({
      propertyId: ['', Validators.required],
      isRequired: [false]
    }));
  }

  removePropertyRow(index: number) {
    this.propertyRows.removeAt(index);
  }

  onSubmit() {
    this.submitAttempted = true;
    this.serverError = '';

    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
      return;
    }

    if (!this.canSubmit) {
      this.formGroup.markAllAsTouched();
      return;
    }

    const formValue = this.formGroup.value;
    const request = {
      name: formValue.name?.trim(),
      description: formValue.description || undefined,
      parentTypeId: formValue.parentTypeId || undefined,
      properties: (formValue.properties || []).map((p, idx) => ({
        propertyId: p.propertyId,
        isRequired: !!p.isRequired,
        displayOrder: idx + 1
      }))
    };

    const save$ = this.updateTypeId
      ? this.assetTypeService.update(this.updateTypeId, request)
      : this.assetTypeService.create(request);

    this.subscriptions.push(
      save$.subscribe({
        next: (res) => {
          this.snackService.snack({ status: 1, message: 'Saved' });
          if (this.dialogRef) {
            this.dialogRef.close(res);
          }
        },
        error: (err) => {
          console.error('Error saving type', err);
          const apiMessage = err?.error?.errors?.name?.[0] || err?.error?.message || err?.error?.title;
          if (err?.status === 409) {
            this.serverError = apiMessage || 'An asset type with this name already exists.';
          } else {
            this.serverError = apiMessage || 'Failed to save type';
          }
          this.snackService.snack({ status: 0, message: this.serverError });
        }
      })
    );
  }

  fetchTypes() {
    this.subscriptions.push(
      this.assetTypeService.getAll().subscribe(types => {
        this.parentTypes = types.filter(t => !t.isArchived);
      })
    );
  }

  fetchProperties() {
    this.subscriptions.push(
      this.assetPropertyService.getAll().subscribe(props => {
        this.availableProperties = props;
        this.propertyMap = (props || []).reduce((acc, curr) => {
          if (curr?.id) {
            acc[curr.id] = curr;
          }
          return acc;
        }, {} as Record<string, AssetProperty>);
      })
    );
  }

  propertyById(propertyId: string | null | undefined): AssetProperty | undefined {
    if (!propertyId) return undefined;
    return this.propertyMap[propertyId];
  }
}