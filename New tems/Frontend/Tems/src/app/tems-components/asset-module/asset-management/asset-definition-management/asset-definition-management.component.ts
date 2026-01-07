import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { AssetDefinitionService } from 'src/app/services/asset-definition.service';
import { AssetTypeService } from 'src/app/services/asset-type.service';
import { AssetDefinition } from 'src/app/models/asset/asset-definition.model';
import { AssetType } from 'src/app/models/asset/asset-type.model';
import { AssetSpecification } from 'src/app/models/asset/asset.model';

@Component({
  selector: 'app-asset-definition-management',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    AgGridAngular
  ],
  templateUrl: './asset-definition-management.component.html',
  styleUrls: ['./asset-definition-management.component.scss']
})
export class AssetDefinitionManagementComponent implements OnInit {
  rowData: AssetDefinition[] = [];
  assetTypes: AssetType[] = [];
  gridApi!: GridApi;
  showCreateModal = false;
  showEditModal = false;
  selectedDefinition: AssetDefinition | null = null;
  createForm!: FormGroup;
  editForm!: FormGroup;
  isSubmitting = false;
  gridReady = false;
  
  createSpecifications: Partial<AssetSpecification>[] = [];
  editSpecifications: Partial<AssetSpecification>[] = [];

  columnDefs: ColDef[] = [
    {
      headerName: 'Name',
      field: 'name',
      flex: 2,
      minWidth: 200,
      cellClass: 'font-medium'
    },
    {
      headerName: 'Type',
      field: 'assetTypeName',
      flex: 1,
      minWidth: 150
    },
    {
      headerName: 'Manufacturer',
      field: 'manufacturer',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => params.value || '—'
    },
    {
      headerName: 'Model',
      field: 'model',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => params.value || '—'
    },
    {
      headerName: 'Usage Count',
      field: 'usageCount',
      flex: 0.7,
      minWidth: 120,
      type: 'numericColumn'
    },
    {
      headerName: 'Status',
      field: 'isArchived',
      flex: 0.7,
      minWidth: 100,
      cellRenderer: (params: any) => {
        const isArchived = params.value;
        const className = isArchived ? 'text-gray-600' : 'text-green-600';
        return `<span class="${className}">${isArchived ? 'Archived' : 'Active'}</span>`;
      }
    },
    {
      headerName: 'Actions',
      flex: 1,
      minWidth: 150,
      cellRenderer: (params: any) => {
        return `
          <button class="action-edit-btn px-2 py-1 text-blue-600 hover:text-blue-800 text-sm mr-2">
            Edit
          </button>
          <button class="action-delete-btn px-2 py-1 text-red-600 hover:text-red-800 text-sm">
            Delete
          </button>
        `;
      },
      onCellClicked: (params) => {
        const target = params.event?.target as HTMLElement;
        if (target.classList.contains('action-edit-btn')) {
          this.editDefinition(params.data);
        } else if (target.classList.contains('action-delete-btn')) {
          this.deleteDefinition(params.data.id);
        }
      }
    }
  ];

  defaultColDef: ColDef = {
    sortable: true,
    filter: true,
    resizable: true,
    flex: 1
  };

  constructor(
    private assetDefinitionService: AssetDefinitionService,
    private assetTypeService: AssetTypeService,
    private fb: FormBuilder
  ) {
    this.createForm = this.fb.group({
      assetTypeId: ['', Validators.required],
      name: ['', Validators.required],
      manufacturer: [''],
      model: [''],
      tags: ['']
    });

    this.editForm = this.fb.group({
      name: ['', Validators.required],
      manufacturer: [''],
      model: [''],
      tags: ['']
    });
  }

  ngOnInit() {
    this.loadDefinitions();
    this.loadTypes();
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
    this.gridReady = true;
    this.gridApi.sizeColumnsToFit();
  }

  loadDefinitions() {
    this.assetDefinitionService.getAll().subscribe({
      next: (definitions) => {
        this.rowData = definitions;
      },
      error: (error) => {
        console.error('Error loading definitions:', error);
      }
    });
  }

  loadTypes() {
    this.assetTypeService.getAll().subscribe({
      next: (types) => {
        this.assetTypes = types.filter(t => !t.isArchived);
      },
      error: (error) => {
        console.error('Error loading types:', error);
      }
    });
  }

  openCreateModal() {
    this.createForm.reset();
    this.createSpecifications = [];
    this.showCreateModal = true;
  }

  closeCreateModal() {
    this.showCreateModal = false;
    this.createForm.reset();
    this.createSpecifications = [];
  }

  addCreateSpecification() {
    this.createSpecifications.push({ propertyId: '', name: '', value: '', dataType: 'string' });
  }

  removeCreateSpecification(index: number) {
    this.createSpecifications.splice(index, 1);
  }

  createDefinition() {
    if (this.createForm.invalid || this.isSubmitting) return;

    this.isSubmitting = true;
    const formValue = this.createForm.value;
    
    const specifications: AssetSpecification[] = this.createSpecifications
      .filter(spec => spec.name && spec.value !== undefined)
      .map(spec => ({
        propertyId: spec.propertyId || spec.name?.toLowerCase().replace(/\s+/g, '_') || '',
        name: spec.name || '',
        value: spec.value,
        dataType: spec.dataType || 'string',
        unit: spec.unit
      }));

    const tags = formValue.tags 
      ? formValue.tags.split(',').map((t: string) => t.trim()).filter((t: string) => t)
      : [];

    const request = {
      assetTypeId: formValue.assetTypeId,
      name: formValue.name,
      manufacturer: formValue.manufacturer || undefined,
      model: formValue.model || undefined,
      specifications: specifications,
      tags: tags
    };

    this.assetDefinitionService.create(request).subscribe({
      next: () => {
        this.loadDefinitions();
        this.closeCreateModal();
        this.isSubmitting = false;
      },
      error: (error) => {
        console.error('Error creating definition:', error);
        this.isSubmitting = false;
      }
    });
  }

  editDefinition(definition: AssetDefinition) {
    this.selectedDefinition = definition;
    this.editForm.patchValue({
      name: definition.name,
      manufacturer: definition.manufacturer || '',
      model: definition.model || '',
      tags: definition.tags.join(', ')
    });
    
    this.editSpecifications = definition.specifications.map(spec => ({
      propertyId: spec.propertyId,
      name: spec.name,
      value: spec.value,
      dataType: spec.dataType,
      unit: spec.unit
    }));
    
    this.showEditModal = true;
  }

  closeEditModal() {
    this.showEditModal = false;
    this.selectedDefinition = null;
    this.editForm.reset();
    this.editSpecifications = [];
  }

  addEditSpecification() {
    this.editSpecifications.push({ propertyId: '', name: '', value: '', dataType: 'string' });
  }

  removeEditSpecification(index: number) {
    this.editSpecifications.splice(index, 1);
  }

  updateDefinition() {
    if (this.editForm.invalid || this.isSubmitting || !this.selectedDefinition) return;

    this.isSubmitting = true;
    const formValue = this.editForm.value;
    
    const specifications: AssetSpecification[] = this.editSpecifications
      .filter(spec => spec.name && spec.value !== undefined)
      .map(spec => ({
        propertyId: spec.propertyId || spec.name?.toLowerCase().replace(/\s+/g, '_') || '',
        name: spec.name || '',
        value: spec.value,
        dataType: spec.dataType || 'string',
        unit: spec.unit
      }));

    const tags = formValue.tags 
      ? formValue.tags.split(',').map((t: string) => t.trim()).filter((t: string) => t)
      : [];

    const request = {
      name: formValue.name,
      manufacturer: formValue.manufacturer || undefined,
      model: formValue.model || undefined,
      specifications: specifications,
      tags: tags
    };

    this.assetDefinitionService.update(this.selectedDefinition.id, request).subscribe({
      next: () => {
        this.loadDefinitions();
        this.closeEditModal();
        this.isSubmitting = false;
      },
      error: (error) => {
        console.error('Error updating definition:', error);
        this.isSubmitting = false;
      }
    });
  }

  deleteDefinition(id: string) {
    if (!confirm('Are you sure you want to delete this definition?')) return;

    this.assetDefinitionService.delete(id).subscribe({
      next: () => {
        this.loadDefinitions();
      },
      error: (error) => {
        console.error('Error deleting definition:', error);
      }
    });
  }
}
