import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { AssetPropertyService } from 'src/app/services/asset-property.service';
import { AssetProperty, PropertyDataType } from 'src/app/models/asset/asset-property.model';

@Component({
  selector: 'app-asset-property-management',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AgGridAngular
  ],
  templateUrl: './asset-property-management.component.html',
  styleUrls: ['./asset-property-management.component.scss']
})
export class AssetPropertyManagementComponent implements OnInit {
  rowData: AssetProperty[] = [];
  gridApi!: GridApi;
  showCreateModal = false;
  showEditModal = false;
  selectedProperty: AssetProperty | null = null;
  createForm!: FormGroup;
  editForm!: FormGroup;
  isSubmitting = false;
  gridReady = false;

  dataTypes = [
    { label: 'String', value: PropertyDataType.String },
    { label: 'Number', value: PropertyDataType.Number },
    { label: 'Boolean', value: PropertyDataType.Boolean },
    { label: 'Date', value: PropertyDataType.Date },
    { label: 'Enum', value: PropertyDataType.Enum }
  ];

  columnDefs: ColDef[] = [
    {
      headerName: 'Name',
      field: 'name',
      flex: 2,
      minWidth: 200,
      cellClass: 'font-medium'
    },
    {
      headerName: 'Category',
      field: 'category',
      flex: 1,
      minWidth: 150
    },
    {
      headerName: 'Data Type',
      field: 'dataType',
      flex: 1,
      minWidth: 120
    },
    {
      headerName: 'Unit',
      field: 'unit',
      flex: 1,
      minWidth: 100
    },
    {
      headerName: 'Enum Values',
      field: 'enumValues',
      flex: 2,
      minWidth: 200,
      valueFormatter: (params) => {
        return params.value ? params.value.join(', ') : '';
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
          this.editProperty(params.data);
        } else if (target.classList.contains('action-delete-btn')) {
          this.deleteProperty(params.data.id);
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
    private assetPropertyService: AssetPropertyService,
    private fb: FormBuilder
  ) {
    this.createForm = this.fb.group({
      name: ['', Validators.required],
      category: ['', Validators.required],
      dataType: [PropertyDataType.String, Validators.required],
      unit: [''],
      enumValues: [''],
      description: ['']
    });

    this.editForm = this.fb.group({
      name: ['', Validators.required],
      category: ['', Validators.required],
      dataType: [PropertyDataType.String, Validators.required],
      unit: [''],
      enumValues: [''],
      description: ['']
    });
  }

  ngOnInit() {
    this.loadProperties();
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
    this.gridReady = true;
    this.gridApi.sizeColumnsToFit();
  }

  loadProperties() {
    this.assetPropertyService.getAll().subscribe({
      next: (properties) => {
        this.rowData = properties;
      },
      error: (error) => {
        console.error('Error loading properties:', error);
      }
    });
  }

  openCreateModal() {
    this.createForm.reset({ dataType: PropertyDataType.String });
    this.showCreateModal = true;
  }

  closeCreateModal() {
    this.showCreateModal = false;
    this.createForm.reset();
  }

  createProperty() {
    if (this.createForm.invalid || this.isSubmitting) return;

    this.isSubmitting = true;
    const formValue = this.createForm.value;
    
    const enumValues = formValue.enumValues 
      ? formValue.enumValues.split(',').map((v: string) => v.trim()).filter((v: string) => v)
      : undefined;

    const request = {
      name: formValue.name,
      category: formValue.category,
      dataType: formValue.dataType,
      unit: formValue.unit || undefined,
      enumValues: enumValues,
      description: formValue.description || undefined
    };

    this.assetPropertyService.create(request).subscribe({
      next: () => {
        this.loadProperties();
        this.closeCreateModal();
        this.isSubmitting = false;
      },
      error: (error) => {
        console.error('Error creating property:', error);
        this.isSubmitting = false;
      }
    });
  }

  editProperty(property: AssetProperty) {
    this.selectedProperty = property;
    this.editForm.patchValue({
      name: property.name,
      category: property.category,
      dataType: property.dataType,
      unit: property.unit || '',
      enumValues: property.enumValues ? property.enumValues.join(', ') : '',
      description: property.description || ''
    });
    this.showEditModal = true;
  }

  closeEditModal() {
    this.showEditModal = false;
    this.selectedProperty = null;
    this.editForm.reset();
  }

  updateProperty() {
    if (this.editForm.invalid || this.isSubmitting || !this.selectedProperty) return;

    this.isSubmitting = true;
    const formValue = this.editForm.value;
    
    const enumValues = formValue.enumValues 
      ? formValue.enumValues.split(',').map((v: string) => v.trim()).filter((v: string) => v)
      : undefined;

    const request = {
      name: formValue.name,
      category: formValue.category,
      dataType: formValue.dataType,
      unit: formValue.unit || undefined,
      enumValues: enumValues,
      description: formValue.description || undefined
    };

    this.assetPropertyService.update(this.selectedProperty.id, request).subscribe({
      next: () => {
        this.loadProperties();
        this.closeEditModal();
        this.isSubmitting = false;
      },
      error: (error) => {
        console.error('Error updating property:', error);
        this.isSubmitting = false;
      }
    });
  }

  deleteProperty(id: string) {
    if (!confirm('Are you sure you want to delete this property?')) return;

    this.assetPropertyService.delete(id).subscribe({
      next: () => {
        this.loadProperties();
      },
      error: (error) => {
        console.error('Error deleting property:', error);
      }
    });
  }
}
