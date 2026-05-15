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
import { AssetType, AssetTypeProperty } from 'src/app/models/asset/asset-type.model';
import { AssetProperty, PropertyDataType } from 'src/app/models/asset/asset-property.model';
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
  availableProperties: AssetProperty[] = [];
  propertyMap: Record<string, AssetProperty> = {};
  formGroup: FormGroup;
  isSubmitting = false;
  assetTypeLocked = false;
  private loadedDefinitionSpecifications: AssetSpecification[] = [];

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
    const options = this.availableProperties
      .filter(p => p?.id)
      .map(p => ({
        value: p.id,
        label: p.name
      }));

    const optionMap = new Map(options.map(option => [option.value, option]));
    this.specsControls.forEach(group => {
      const propertyId = group.get('propertyId')?.value;
      const propertyName = group.get('name')?.value;
      if (propertyId && !optionMap.has(propertyId)) {
        optionMap.set(propertyId, {
          value: propertyId,
          label: propertyName || propertyId
        });
      }
    });

    return Array.from(optionMap.values());
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

    this.watchAssetTypeSelection();
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
  }

  fetchTypes() {
    this.subscriptions.push(
      this.assetTypeService.getAll().subscribe(types => {
        this.assetTypes = types.filter(t => !t.isArchived);
        const selectedTypeId = this.formGroup.getRawValue().assetTypeId;
        if (selectedTypeId) {
          this.hydrateSpecificationsForType(selectedTypeId, this.loadedDefinitionSpecifications);
        }
      })
    );
  }

  fetchProperties() {
    this.subscriptions.push(
      this.assetPropertyService.getAll().subscribe(props => {
        this.availableProperties = props;
        this.propertyMap = (props || []).reduce((acc, property) => {
          acc[property.id] = property;
          return acc;
        }, {} as Record<string, AssetProperty>);
        this.refreshSpecificationMetadata();
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

  getSpecificationUnit(spec: FormGroup): string {
    return spec.get('unit')?.value || 'No unit';
  }

  isSpecificationRequired(spec: FormGroup): boolean {
    return !!spec.get('isRequired')?.value;
  }

  private watchAssetTypeSelection() {
    this.subscriptions.push(
      this.formGroup.get('assetTypeId')!.valueChanges.subscribe(assetTypeId => {
        if (this.isUpdateMode) return;

        if (!assetTypeId) {
          this.specs.clear();
          return;
        }

        this.hydrateSpecificationsForType(assetTypeId);
      })
    );
  }

  private buildSpecificationGroup(spec?: Partial<AssetSpecification> & { isRequired?: boolean }): FormGroup {
    const group = this.fb.group({
      propertyId: [spec?.propertyId || '', Validators.required],
      name: [spec?.name || ''],
      value: [spec?.value ?? ''],
      dataType: [spec?.dataType || PropertyDataType.String],
      unit: [spec?.unit || ''],
      isRequired: [spec?.isRequired || false]
    });

    this.applySpecificationMetadata(group, spec?.propertyId || '', spec);
    this.registerPropertySync(group);

    return group;
  }

  private registerPropertySync(group: FormGroup) {
    this.subscriptions.push(
      group.get('propertyId')!.valueChanges.subscribe(propertyId => {
        this.applySpecificationMetadata(group, propertyId);
      })
    );
  }

  private applySpecificationMetadata(
    group: FormGroup,
    propertyId: string,
    existingSpec?: Partial<AssetSpecification> & { isRequired?: boolean }
  ) {
    const property = propertyId ? this.propertyMap[propertyId] : undefined;
    const typeProperty = propertyId ? this.getSelectedTypeProperty(propertyId) : undefined;
    const valueControl = group.get('value');
    const isRequired = existingSpec?.isRequired ?? typeProperty?.isRequired ?? false;
    const defaultValue = typeProperty?.defaultValue ?? '';

    group.patchValue({
      name: existingSpec?.name || typeProperty?.propertyName || property?.name || '',
      dataType: existingSpec?.dataType || property?.dataType || PropertyDataType.String,
      unit: existingSpec?.unit || typeProperty?.validation?.unit || property?.unit || '',
      isRequired
    }, { emitEvent: false });

    if (valueControl) {
      valueControl.setValidators(isRequired ? [Validators.required] : []);
      if ((valueControl.value === null || valueControl.value === undefined || valueControl.value === '') && existingSpec?.value === undefined && defaultValue) {
        valueControl.setValue(defaultValue, { emitEvent: false });
      }
      valueControl.updateValueAndValidity({ emitEvent: false });
    }
  }

  private getSelectedTypeProperty(propertyId: string): AssetTypeProperty | undefined {
    const selectedTypeId = this.formGroup.getRawValue().assetTypeId;
    const selectedType = this.assetTypes.find(type => type.id === selectedTypeId);
    return selectedType?.properties?.find(property => property.propertyId === propertyId);
  }

  private hydrateSpecificationsForType(assetTypeId: string, existingSpecs: AssetSpecification[] = []) {
    const assetType = this.assetTypes.find(type => type.id === assetTypeId);

    if (!assetType) {
      this.subscriptions.push(
        this.assetTypeService.getById(assetTypeId).subscribe({
          next: (type) => {
            const existingIndex = this.assetTypes.findIndex(item => item.id === type.id);
            if (existingIndex >= 0) {
              this.assetTypes[existingIndex] = type;
            } else {
              this.assetTypes = [...this.assetTypes, type];
            }
            this.hydrateSpecificationsForType(assetTypeId, existingSpecs);
          },
          error: (error) => {
            console.error('Error loading asset type:', error);
          }
        })
      );
      return;
    }

    const existingSpecMap = new Map(existingSpecs.map(spec => [spec.propertyId, spec]));
    const orderedTypeSpecs = [...(assetType.properties || [])]
      .sort((left, right) => (left.displayOrder ?? 0) - (right.displayOrder ?? 0))
      .map(typeProperty => {
        const existingSpec = existingSpecMap.get(typeProperty.propertyId);
        existingSpecMap.delete(typeProperty.propertyId);

        return this.buildSpecificationGroup({
          propertyId: typeProperty.propertyId,
          name: existingSpec?.name || typeProperty.propertyName,
          value: existingSpec?.value ?? typeProperty.defaultValue ?? '',
          dataType: existingSpec?.dataType,
          unit: existingSpec?.unit || typeProperty.validation?.unit,
          isRequired: typeProperty.isRequired
        });
      });

    const extraSpecGroups = Array.from(existingSpecMap.values()).map(spec => this.buildSpecificationGroup(spec));

    this.specs.clear();
    [...orderedTypeSpecs, ...extraSpecGroups].forEach(group => this.specs.push(group));
  }

  private refreshSpecificationMetadata() {
    this.specsControls.forEach(group => {
      this.applySpecificationMetadata(group, group.get('propertyId')?.value);
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
          this.loadedDefinitionSpecifications = definition.specifications || [];
          this.hydrateSpecificationsForType(definition.assetTypeId, this.loadedDefinitionSpecifications);
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
      propertyId: s.propertyId,
      name: s.name,
      value: s.value,
      dataType: s.dataType,
      unit: s.unit || undefined
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
