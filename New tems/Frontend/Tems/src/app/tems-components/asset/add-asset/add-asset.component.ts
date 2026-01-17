import { Component, EventEmitter, Input, OnInit, Output, Inject, Optional, HostListener, ElementRef, ChangeDetectorRef } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { trigger, transition, style, animate } from '@angular/animations';
import { AddEquipment } from 'src/app/models/asset/add-asset.model';
import { DefinitionService } from 'src/app/services/definition.service';
import { AssetDefinitionService } from 'src/app/services/asset-definition.service';
import { AssetDefinition } from 'src/app/models/asset/asset-definition.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Definition } from '../../../models/asset/add-definition.model';
import { DialogService } from '../../../services/dialog.service';
import { FormlyParserService } from '../../../services/formly-parser.service';
import { SnackService } from '../../../services/snack.service';
import { ErrorMessageService } from '../../../services/error-message.service';
import { TypeService } from '../../../services/type.service';
import { AddTypeComponent } from '.././add-type/add-type.component';
import { BulkUploadComponent } from '../bulk-upload/bulk-upload.component';
import { SICFileUploadResult } from './../../../models/asset/bulk-upload-result.model';
import { FormlyData } from './../../../models/formly/formly-data.model';
import { IOption } from './../../../models/option.model';
import { AssetService } from './../../../services/asset.service';
import { AddDefinitionComponent } from './../add-definition/add-definition.component';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TranslateModule } from '@ngx-translate/core';
import { TEMS_FORMS_IMPORTS } from 'src/app/shared/constants/tems-forms-imports.const';
import { BulkUploadResultsComponent } from '../bulk-upload-results/bulk-upload-results.component';

@Component({
  selector: 'app-add-asset',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatStepperModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    TranslateModule,
    BulkUploadResultsComponent,
    ...TEMS_FORMS_IMPORTS
  ],
  templateUrl: './add-asset.component.html',
  styleUrls: ['./add-asset.component.scss'],
  providers: [],
  animations: [
    trigger('expandCollapse', [
      transition(':enter', [
        style({ height: '0', opacity: '0', overflow: 'hidden' }),
        animate('200ms ease-in-out', style({ height: '*', opacity: '1' }))
      ]),
      transition(':leave', [
        style({ height: '*', opacity: '1', overflow: 'hidden' }),
        animate('200ms ease-in-out', style({ height: '0', opacity: '0' }))
      ])
    ])
  ]
})
export class AddAssetComponent extends TEMSComponent implements OnInit {

  // BEFREE: Divide this component across multiple smaller and more maintanable components

  @Input() updateEquipmentId: string;
  @Input() updateAssetDefinitionId: string;

  @Output() done = new EventEmitter();
  @Output() goBack = new EventEmitter();

  updateEquipment: AddEquipment;

  types: IOption[] = [];
  selectedType: string;

  definitionsOfType: IOption[];
  selectedDefinition: IOption;
  selectedFullDefinition: AssetDefinition = undefined;
  originalDefinition: AssetDefinition = undefined; // Store original for restore

  // UI State
  isTypeDropdownOpen = false;
  isDefinitionDropdownOpen = false;
  isStatusDropdownOpen = false;
  isDefinitionPreviewExpanded = false;
  isProcurementExpanded = false;
  isAllocationExpanded = false;
  
  typeSearchText = '';
  definitionSearchText = '';
  
  // Editing state
  isEditingAssetName = false;
  editingSpecificationIndex: number | null = null;
  editedAssetName = '';
  editedSpecificationValue: any = null;

  private focusElementById(elementId: string) {
    setTimeout(() => {
      const el = document.getElementById(elementId) as HTMLInputElement | HTMLSelectElement | null;
      if (el) {
        el.focus();
      }
    });
  }
  
  // Status selection
  selectedStatus: 'new' | 'used' | 'defect' | undefined = undefined;
  statusOptions = [
    { value: 'new' as const, label: 'New', description: 'Brand new, never used' },
    { value: 'used' as const, label: 'Used', description: 'Previously used, functional' },
    { value: 'defect' as const, label: 'Defect', description: 'Non-functional or damaged' }
  ] as const;

  // For selecting type and definition
  formGroup = new FormGroup({
    assetType: new FormControl(),
    assetDefinition: new FormControl(),
    properties: new FormControl(),
  });

  // For providing equipment data
  formlyData = new FormlyData();

  bulkUploadResults: SICFileUploadResult[];

  constructor(
    private assetService: AssetService,
    private typeService: TypeService,
    private definitionService: DefinitionService,
    private assetDefinitionService: AssetDefinitionService,
    private formlyParserService: FormlyParserService,
    private dialogService: DialogService,
    private snackService: SnackService,
    private errorMessageService: ErrorMessageService,
    private router: Router,
    private elementRef: ElementRef,
    private cdr: ChangeDetectorRef,
    @Optional() public dialogRef: MatDialogRef<AddAssetComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }

  ngOnInit(): void {
    console.log('AddAssetComponent ngOnInit called, updateEquipmentId:', this.updateEquipmentId);
    if (this.updateEquipmentId != undefined) {
      this.edit();
      return;
    }

    this.fetchTypes();
  }

  fetchTypes() {
    console.log('Fetching asset types from backend...');
    this.subscriptions.push(
      this.typeService.getAllAutocompleteOptions()
        .subscribe({
          next: (response) => {
            console.log('Asset types received:', response);
            if (this.snackService.snackIfError(response))
              return;

            this.types = response;
            console.log('Asset types set:', this.types);
          },
          error: (error) => {
            console.error('Error fetching asset types:', error);
          }
        }));
  }

  // Fetches all of selected type's definitions (called after selecting a type in order to display 
  // definition options)
  fetchDefinitionsOfType() {

    if (this.types.find(q => q.value == this.selectedType) == undefined) {
      this.definitionsOfType = undefined;
      return;
    }

    this.subscriptions.push(
      this.definitionService.getDefinitionsOfType(this.selectedType)
        .subscribe(response => {
          if (this.snackService.snackIfError(response))
            return;

          this.definitionsOfType = response;
        }))
  }

  // Cleans the forms and retrieves the definitions of the newly selected type
  onTypeChanged(eventData) {
    if (eventData.value == undefined)
      return;

    this.wipeAddAssetFormly();
    this.fetchDefinitionsOfType();
  }

  // Retrieves the full definition and build the formly form according to the data provided
  // by the definition
  onDefinitionChanged(eventData) {
    if (eventData.value == undefined)
      return;

    // reset form visibility while loading
    this.formlyData.isVisible = false;

    this.subscriptions.push(
      this.assetDefinitionService.getById(eventData.value)
        .subscribe({
          next: (result: any) => {
            // Unwrap the assetDefinition property if it exists
            this.selectedFullDefinition = result.assetDefinition || result;
            // Store original definition for restore
            this.originalDefinition = JSON.parse(JSON.stringify(this.selectedFullDefinition));
            
            // Initialize editing state
            this.editedAssetName = this.selectedFullDefinition.name || '';
            this.isEditingAssetName = false;
            this.editingSpecificationIndex = null;
            this.editedSpecificationValue = null;
            
            // Show the form
            this.formlyData.isVisible = true;
            this.cdr.markForCheck();
          },
          error: (err) => {
            console.error('Error fetching definition:', err);
            this.snackService.snack({ message: 'Failed to load asset definition', status: 0 });
            this.formlyData.isVisible = true;
          }
        }));
  }

  // Calls formly parser service in order to build formly form structure
  // and sets the formly data object 
  createAddAssetFormly() {
    // Adapt the new AssetDefinition model to the legacy AddEquipment structure expected by the formly parser
    const legacyDefinition: any = {
      id: this.selectedFullDefinition.id,
      identifier: this.selectedFullDefinition.name,
      price: 0,
      currency: 'usd',
      assetType: { label: this.selectedFullDefinition.assetTypeName ?? 'Asset' },
      children: [],
      properties: this.selectedFullDefinition.specifications || []
    };

    const addEquipment: any = {
      definition: legacyDefinition,
      temsid: '',
      serialNumber: '',
      price: legacyDefinition.price,
      currency: legacyDefinition.currency,
      description: '',
      purchaseDate: new Date(),
      isDefect: false,
      isUsed: false,
      children: []
    };

    try {
      const formlyFields = this.formlyParserService.parseAddEquipment(addEquipment, this.updateEquipmentId != undefined);
      this.formlyData.fields = formlyFields;

      this.formlyData.model = {
        assetDefinitionID: this.selectedFullDefinition.id,
        identifier: this.selectedFullDefinition.name,
        temsid: '',
        serialNumber: '',
        price: legacyDefinition.price,
        currency: legacyDefinition.currency,
        purchaseDate: new Date().toISOString().split('T')[0],
        description: '',
        isUsed: false,
        isDefect: false
      };
    } catch (err) {
      console.error('Failed to build formly form', err);
      this.snackService.snack({ message: 'Form generation failed for this definition', status: 0 });
    } finally {
      this.formlyData.isVisible = true;
    }
  }

  // Wipes collected data except equipment type
  wipeAddAssetFormly() {
    this.selectedDefinition = { value: '', label: '' } as IOption;
    this.selectedFullDefinition = undefined;
    this.formlyData = new FormlyData();
  }

  openAddDefinition() {
    this.dialogService.openDialog(
      AddDefinitionComponent,
      [{ label: "typeId", value: this.selectedType }],
      () => {
        this.fetchDefinitionsOfType();
      });
  }

  openAddType() {
    this.dialogService.openDialog(
      AddTypeComponent,
      undefined,
      () => {
        this.fetchTypes();
      });
  }

  back() {
    this.goBack.emit();
  }

  // Opens the BulkUploadComponent in a dialog
  bulkUpload() {
    let dialogRef = this.dialogService.openDialog(BulkUploadComponent);

    dialogRef.afterClosed().subscribe(() => {
      this.bulkUploadResults = dialogRef.componentInstance["uploadResults"];
    });
  }

  // Clear bulk upload results
  clearResults() {
    this.bulkUploadResults = undefined;
  }

  // Gets updateEquipment's definition and build the formly-form accordingly
  edit() {
    if (this.updateAssetDefinitionId == undefined)
      return;

    this.subscriptions.push(
      this.assetService.getFullDefinition(this.updateAssetDefinitionId)
        .subscribe(result => {
          if (this.snackService.snackIfError(result))
            return;

          this.selectedFullDefinition = result;

          this.createAddAssetFormly();
          this.fetchEquipmentData();
        })
    )
  }

  // Fetches equipment's information (for update case) and builds the formly model accordingly
  fetchEquipmentData() {
    this.subscriptions.push(
      this.assetService.getEquipmentToUpdate(this.updateEquipmentId)
        .subscribe(result => {
          if (this.snackService.snackIfError(result))
            return;

          this.formlyData.wipeModel();

          this.formlyData.model.assetDefinitionID = this.selectedFullDefinition.id;
          this.formlyData.model.identifier = this.selectedFullDefinition.name;
          this.formlyData.model.temsid = result.temsid;
          this.formlyData.model.serialNumber = result.serialNumber;
          this.formlyData.model.isDefect = result.isDefect;
          this.formlyData.model.isUsed = result.isUsed;
          this.formlyData.model.description = result.description;
          this.formlyData.model.price = result.price;
          this.formlyData.model.currency = (result.currency != undefined) ? result.currency : 'USD';
          this.formlyData.model.purchaseDate = result.purchaseDate.toString().split('T')[0];
        })
    )
  }

  // Build an instance of AddAssetViewModel and sends it the server
  onSubmit(model) {
    if (!this.isFormValid()) {
      this.snackService.snack({ status: 0, message: 'Please complete all required fields' });
      return;
    }

    // Check if definition has been customized
    const isCustomized = this.isDefinitionEdited();
    
    // Prepare custom specifications if definition was edited
    const customSpecifications = isCustomized && this.selectedFullDefinition?.specifications
      ? this.selectedFullDefinition.specifications.map(spec => ({
          propertyId: spec.propertyId || '',
          name: spec.name,
          value: spec.value,
          dataType: spec.dataType,
          unit: spec.unit || null
        }))
      : null;

    const assetData = {
      serialNumber: this.formlyData.model.serialNumber,
      assetTag: this.formlyData.model.temsid,
      status: this.selectedStatus || 'used',
      assetTypeId: this.selectedType,
      assetTypeName: this.selectedFullDefinition?.assetTypeName || '',
      definitionId: this.selectedDefinition?.value || '',
      definitionName: this.selectedFullDefinition?.name || '',
      manufacturer: this.selectedFullDefinition?.manufacturer || '',
      model: this.selectedFullDefinition?.model || '',
      tags: this.selectedFullDefinition?.tags || [],
      customizeDefinition: isCustomized,
      customSpecifications: customSpecifications,
      notes: this.formlyData.model.description || '',
      createdBy: 'current-user' // TODO: Get from auth service
    };

    const endPoint = this.updateEquipmentId == undefined
      ? this.assetService.createAsset(assetData)
      : this.assetService.updateEquipment(model); // Keep old logic for update

    this.subscriptions.push(
      endPoint
        .subscribe({
          next: (result) => {
            if (this.updateEquipmentId == undefined) {
              this.snackService.snack({ status: 1, message: 'Asset created successfully!' });
              // Clear mandatory fields
              this.formlyData.model.temsid = '';
              this.formlyData.model.serialNumber = '';
              this.selectedStatus = undefined;
              // Keep customized definition - user can reset manually if needed
            } else {
              this.snackService.snack({ status: 1, message: 'Asset updated successfully!' });
            }
          },
          error: (error) => {
            if (error.status === 409) {
              const errorMessage = this.errorMessageService.getErrorMessage(409);
              this.snackService.snack({ status: 0, message: errorMessage });
            } else {
              const errorMessage = this.errorMessageService.getErrorMessage(
                error.status,
                error.error?.message || 'An error occurred while saving the asset'
              );
              this.snackService.snack({ status: 0, message: errorMessage });
            }
          }
        }));
  }

  // UI Control Methods
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    const clickedInside = this.elementRef.nativeElement.contains(target);
    
    if (!clickedInside) {
      this.isTypeDropdownOpen = false;
      this.isDefinitionDropdownOpen = false;
      this.isStatusDropdownOpen = false;
      this.typeSearchText = '';
      this.definitionSearchText = '';
    }
  }

  toggleTypeDropdown() {
    this.isTypeDropdownOpen = !this.isTypeDropdownOpen;
    if (!this.isTypeDropdownOpen) {
      this.typeSearchText = '';
    }
  }

  toggleDefinitionDropdown() {
    this.isDefinitionDropdownOpen = !this.isDefinitionDropdownOpen;
    if (!this.isDefinitionDropdownOpen) {
      this.definitionSearchText = '';
    }
  }

  toggleStatusDropdown() {
    this.isStatusDropdownOpen = !this.isStatusDropdownOpen;
  }

  toggleDefinitionPreview() {
    this.isDefinitionPreviewExpanded = !this.isDefinitionPreviewExpanded;
  }
  
  startEditingAssetName() {
    this.editedAssetName = this.selectedFullDefinition?.name ?? '';
    this.isEditingAssetName = true;
    this.cdr.detectChanges();
    this.focusElementById('asset-name-input');
  }
  
  saveAssetName() {
    if (this.editedAssetName.trim()) {
      this.selectedFullDefinition = { ...this.selectedFullDefinition, name: this.editedAssetName };
      this.isEditingAssetName = false;
    }
  }
  
  cancelEditingAssetName() {
    this.isEditingAssetName = false;
    this.editedAssetName = this.selectedFullDefinition.name || '';
  }
  
  startEditingSpecification(index: number) {
    this.editingSpecificationIndex = index;
    this.editedSpecificationValue = this.selectedFullDefinition?.specifications?.[index]?.value ?? '';
    this.cdr.detectChanges();
    this.focusElementById(`spec-input-${index}`);
  }
  
  saveSpecification(index: number) {
    if (this.editedSpecificationValue !== null && this.editedSpecificationValue !== undefined) {
      const updatedSpecs = [...this.selectedFullDefinition.specifications];
      updatedSpecs[index] = { ...updatedSpecs[index], value: this.editedSpecificationValue };
      this.selectedFullDefinition = {
        ...this.selectedFullDefinition,
        specifications: updatedSpecs
      };
    }
    this.editingSpecificationIndex = null;
    this.editedSpecificationValue = null;
  }
  
  cancelEditingSpecification() {
    this.editingSpecificationIndex = null;
    this.editedSpecificationValue = null;
  }
  
  isDefinitionEdited(): boolean {
    if (!this.selectedFullDefinition || !this.originalDefinition) return false;
    return this.selectedFullDefinition.name !== this.originalDefinition.name ||
           JSON.stringify(this.selectedFullDefinition.specifications) !== JSON.stringify(this.originalDefinition.specifications);
  }
  
  restoreDefinition() {
    if (this.originalDefinition) {
      this.selectedFullDefinition = JSON.parse(JSON.stringify(this.originalDefinition));
      this.editedAssetName = this.selectedFullDefinition.name || '';
      this.isEditingAssetName = false;
      this.editingSpecificationIndex = null;
      this.editedSpecificationValue = null;
      this.cdr.markForCheck();
    }
  }

  toggleProcurement() {
    this.isProcurementExpanded = !this.isProcurementExpanded;
  }

  toggleAllocation() {
    this.isAllocationExpanded = !this.isAllocationExpanded;
  }

  selectType(typeId: string) {
    this.selectedType = typeId;
    this.isTypeDropdownOpen = false;
    this.typeSearchText = '';
    this.onTypeChanged({ value: typeId });
  }

  selectDefinition(definitionId: string) {
    const definition = this.definitionsOfType.find(d => d.value === definitionId);
    this.selectedDefinition = definition;
    this.isDefinitionDropdownOpen = false;
    this.definitionSearchText = '';
    this.onDefinitionChanged({ value: definitionId });
  }

  getFilteredTypes(): IOption[] {
    if (!this.typeSearchText) {
      return this.types;
    }
    return this.types.filter(t => 
      t.label.toLowerCase().includes(this.typeSearchText.toLowerCase())
    );
  }

  getFilteredDefinitions(): IOption[] {
    if (!this.definitionsOfType) {
      return [];
    }
    if (!this.definitionSearchText) {
      return this.definitionsOfType;
    }
    return this.definitionsOfType.filter(d => 
      d.label.toLowerCase().includes(this.definitionSearchText.toLowerCase())
    );
  }

  getTypeName(typeId: string): string {
    const type = this.types.find(t => t.value === typeId);
    return type ? type.label : '';
  }

  getDefinitionName(defId: string): string {
    const def = this.definitionsOfType?.find(d => d.value === defId);
    return def ? def.label : '';
  }

  selectStatus(status: 'new' | 'used' | 'defect' | undefined) {
    this.selectedStatus = status;
    this.isStatusDropdownOpen = false;
    if (this.formlyData.model && status) {
      this.formlyData.model.isUsed = status !== 'new';
      this.formlyData.model.isDefect = status === 'defect';
    }
  }

  getStatusLabel(status: 'new' | 'used' | 'defect'): string {
    const statusOption = this.statusOptions.find(s => s.value === status);
    return statusOption ? statusOption.label : '';
  }

  isFormValid(): boolean {
    return !!this.selectedType && 
           !!this.selectedDefinition && 
           !!this.formlyData.model.temsid?.trim() && 
           !!this.formlyData.model.serialNumber?.trim() && 
           !!this.selectedStatus;
  }

  openInFullPage() {
    if (this.dialogRef) {
      this.dialogRef.close();
    }
    this.router.navigate(['/asset/add']);
  }
}