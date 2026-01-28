import { Component, Inject, Input, OnInit, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { SnackService } from 'src/app/services/snack.service';
import { AssetDefinitionService } from 'src/app/services/asset-definition.service';
import { AssetTypeService } from 'src/app/services/asset-type.service';
import { AssetPropertyService } from 'src/app/services/asset-property.service';
import { AssetSpecification } from 'src/app/models/asset/asset.model';
import { AssetType } from 'src/app/models/asset/asset-type.model';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

@Component({
  selector: 'app-add-definition',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CustomSelectComponent
  ],
  templateUrl: './add-definition.component.html',
  styleUrls: ['./add-definition.component.scss']
})

export class AddDefinitionComponent extends TEMSComponent implements OnInit {
  updateDefinitionId: string;
  @Input() typeId: string;
  assetTypes: AssetType[] = [];
  availableProperties: any[] = [];
  formGroup: FormGroup;
  isSubmitting = false;
  assetTypeLocked = false;

  get specs(): FormArray {
    return this.formGroup.get('specifications') as FormArray;
  }

  get specsControls(): FormGroup[] {
    return this.specs.controls as FormGroup[];
  }

  get assetTypeOptions(): SelectOption[] {
    return this.assetTypes.map(t => ({ value: t.id, label: t.name }));
  }

  get propertyOptions(): SelectOption[] {
    return this.availableProperties
      .filter(p => (p as any)?.id || (p as any)?.propertyId)
      .map(p => ({
        value: (p as any).id ?? (p as any).propertyId,
        label: p.name
      }));
  }

  constructor(
    private fb: FormBuilder,
    private assetDefinitionService: AssetDefinitionService,
    private assetTypeService: AssetTypeService,
    private assetPropertyService: AssetPropertyService,
    private snackService: SnackService,
    @Optional() public dialogRef: MatDialogRef<AddDefinitionComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();
    this.updateDefinitionId = this.updateDefinitionId ?? this.dialogData?.updateDefinitionId;
    this.typeId = this.typeId ?? this.dialogData?.typeId;

    this.formGroup = this.fb.group({
      assetTypeId: ['', Validators.required],
      name: ['', Validators.required],
      manufacturer: [''],
      model: [''],
      tags: [''],
      specifications: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.typeId = this.typeId ?? this.dialogData?.typeId;
    this.updateDefinitionId = this.updateDefinitionId ?? this.dialogData?.updateDefinitionId;

    this.fetchTypes();
    this.fetchProperties();

    if (this.typeId) {
      this.formGroup.patchValue({ assetTypeId: this.typeId });
      this.lockAssetType();
    }

    if (this.updateDefinitionId) {
      this.lockAssetType();
      this.loadDefinition(this.updateDefinitionId);
      return;
    }

    this.addSpecification();
  }

  fetchTypes() {
    this.subscriptions.push(
      this.assetTypeService.getAll().subscribe(types => {
        this.assetTypes = types.filter(t => !t.isArchived);
      })
    );
  }

  fetchProperties() {
    this.subscriptions.push(
      this.assetPropertyService.getAll().subscribe(props => {
        this.availableProperties = props;
      })
    );
  }

  get isUpdateMode(): boolean {
    return !!this.updateDefinitionId;
  }

  private lockAssetType() {
    this.assetTypeLocked = true;
    this.formGroup.get('assetTypeId')?.disable();
  }

  addSpecification() {
    this.specs.push(this.buildSpecificationGroup());
  }

  removeSpecification(index: number) {
    this.specs.removeAt(index);
  }

  private buildSpecificationGroup(spec?: AssetSpecification): FormGroup {
    return this.fb.group({
      propertyId: [spec?.propertyId || ''],
      name: [spec?.name || '', Validators.required],
      value: [spec?.value ?? '', Validators.required],
      dataType: [spec?.dataType || 'string'],
      unit: [spec?.unit || ''],
      isRequired: [spec?.isRequired || false]
    });
  }

  private loadDefinition(id: string) {
    this.subscriptions.push(
      this.assetDefinitionService.getById(id).subscribe({
        next: (definition) => {
          this.formGroup.patchValue({
            assetTypeId: definition.assetTypeId,
            name: definition.name,
            manufacturer: definition.manufacturer || '',
            model: definition.model || '',
            tags: definition.tags?.join(', ') || ''
          });

          this.specs.clear();
          if (definition.specifications?.length) {
            definition.specifications.forEach(spec => this.specs.push(this.buildSpecificationGroup(spec)));
          } else {
            this.addSpecification();
          }
        },
        error: (err) => {
          console.error('Error loading definition', err);
          this.snackService.snack({ status: 0, message: 'Failed to load definition' });
        }
      })
    );
  }

  onSubmit() {
    if (this.formGroup.invalid || this.isSubmitting) {
      this.formGroup.markAllAsTouched();
      return;
    }

    const formValue = this.formGroup.getRawValue();
    const specifications: AssetSpecification[] = (formValue.specifications || []).map((s: any) => ({
      propertyId: s.propertyId || s.name.toLowerCase().replace(/\s+/g, '_'),
      name: s.name,
      value: s.value,
      dataType: s.dataType,
      unit: s.unit || undefined,
      isRequired: s.isRequired || false
    }));

    const tags = formValue.tags
      ? formValue.tags.split(',').map((t: string) => t.trim()).filter((t: string) => t)
      : [];

    const createRequest = {
      assetTypeId: formValue.assetTypeId,
      name: formValue.name,
      manufacturer: formValue.manufacturer || undefined,
      model: formValue.model || undefined,
      specifications,
      tags
    };

    const updateRequest = {
      name: formValue.name,
      manufacturer: formValue.manufacturer || undefined,
      model: formValue.model || undefined,
      specifications,
      tags
    };

    const save$ = this.isUpdateMode
      ? this.assetDefinitionService.update(this.updateDefinitionId, updateRequest)
      : this.assetDefinitionService.create(createRequest);

    this.isSubmitting = true;

    this.subscriptions.push(
      save$.subscribe({
        next: (res) => {
          this.snackService.snack({ status: 1, message: 'Saved' });
          if (this.dialogRef) {
            this.dialogRef.close(res);
          }
        },
        error: (err) => {
          console.error('Error saving definition', err);
          this.snackService.snack({ status: 0, message: 'Failed to save definition' });
        }
      }).add(() => {
        this.isSubmitting = false;
      })
    );
  }
}
