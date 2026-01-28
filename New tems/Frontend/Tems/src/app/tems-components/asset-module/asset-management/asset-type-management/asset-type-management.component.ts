import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormArray } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { AssetTypeService } from 'src/app/services/asset-type.service';
import { AssetPropertyService } from 'src/app/services/asset-property.service';
import { ThemeService } from 'src/app/services/theme.service';
import { AssetType, AssetTypePropertyRequest } from 'src/app/models/asset/asset-type.model';
import { AssetProperty } from 'src/app/models/asset/asset-property.model';
import { AddTypeComponent } from '../../../asset/add-type/add-type.component';

@Component({
  selector: 'app-asset-type-management',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    AgGridAngular,
    MatDialogModule
  ],
  templateUrl: './asset-type-management.component.html',
  styleUrls: ['./asset-type-management.component.scss']
})
export class AssetTypeManagementComponent implements OnInit {
  rowData: AssetType[] = [];
  availableProperties: AssetProperty[] = [];
  parentTypes: AssetType[] = [];
  gridApi!: GridApi;
  showEditModal = false;
  selectedType: AssetType | null = null;
  editForm!: FormGroup;
  isSubmitting = false;
  gridReady = false;

  columnDefs: ColDef[] = [
    {
      headerName: 'Name',
      field: 'name',
      flex: 2,
      minWidth: 200,
      cellClass: 'font-medium'
    },
    {
      headerName: 'Parent Type',
      field: 'parentTypeName',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => params.value || '—'
    },
    {
      headerName: 'Properties',
      field: 'properties',
      flex: 1,
      minWidth: 100,
      valueFormatter: (params) => params.value?.length || 0
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
          this.editType(params.data);
        } else if (target.classList.contains('action-delete-btn')) {
          this.deleteType(params.data.id);
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

  get gridThemeClass(): string {
    return this.themeService.isDarkMode ? 'ag-theme-quartz-auto-dark' : 'ag-theme-quartz';
  }

  constructor(
    private assetTypeService: AssetTypeService,
    private assetPropertyService: AssetPropertyService,
    private fb: FormBuilder,
    private dialog: MatDialog,
    private themeService: ThemeService
  ) {
    this.editForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      parentTypeId: [''],
      selectedProperties: this.fb.array([])
    });
  }

  get editSelectedProperties() {
    return this.editForm.get('selectedProperties') as FormArray;
  }

  ngOnInit() {
    this.loadTypes();
    this.loadProperties();
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
    this.gridReady = true;
    this.gridApi.sizeColumnsToFit();
  }

  loadTypes() {
    this.assetTypeService.getAll().subscribe({
      next: (types) => {
        this.rowData = types;
        this.parentTypes = types.filter(t => !t.isArchived);
      },
      error: (error) => {
        console.error('Error loading types:', error);
      }
    });
  }

  loadProperties() {
    this.assetPropertyService.getAll().subscribe({
      next: (properties) => {
        this.availableProperties = properties;
      },
      error: (error) => {
        console.error('Error loading properties:', error);
      }
    });
  }

  openCreateModal() {
    const dialogRef = this.dialog.open(AddTypeComponent, {
      width: '900px',
      maxHeight: '90vh',
      panelClass: 'custom-dialog-container'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result !== undefined) {
        this.loadTypes();
        this.loadProperties();
      }
    });
  }

  editType(type: AssetType) {
    this.selectedType = type;
    this.editForm.patchValue({
      name: type.name,
      description: type.description || '',
      parentTypeId: type.parentTypeId || ''
    });

    this.editSelectedProperties.clear();
    (type.properties || []).forEach((p, index) => {
      const propertyGroup = this.fb.group({
        propertyId: [p.propertyId, Validators.required],
        isRequired: [p.isRequired],
        displayOrder: [p.displayOrder ?? index + 1],
        defaultValue: [p.defaultValue || '']
      });
      this.editSelectedProperties.push(propertyGroup);
    });

    this.showEditModal = true;
  }

  closeEditModal() {
    this.showEditModal = false;
    this.selectedType = null;
    this.editForm.reset();
    this.editSelectedProperties.clear();
    this.isSubmitting = false;
  }
  addPropertyToEdit() {
    const propertyGroup = this.fb.group({
      propertyId: ['', Validators.required],
      isRequired: [false],
      displayOrder: [this.editSelectedProperties.length + 1],
      defaultValue: ['']
    });
    this.editSelectedProperties.push(propertyGroup);
  }

  removePropertyFromEdit(index: number) {
    this.editSelectedProperties.removeAt(index);
  }

  updateType() {
    if (this.editForm.invalid || this.isSubmitting || !this.selectedType) return;

    this.isSubmitting = true;
    const formValue = this.editForm.value;
    
    const properties: AssetTypePropertyRequest[] = formValue.selectedProperties.map((p: any) => ({
      propertyId: p.propertyId,
      isRequired: p.isRequired,
      displayOrder: p.displayOrder,
      defaultValue: p.defaultValue || undefined
    }));

    const request = {
      name: formValue.name,
      description: formValue.description || undefined,
      parentTypeId: formValue.parentTypeId || undefined,
      properties: properties
    };

    this.assetTypeService.update(this.selectedType.id, request).subscribe({
      next: () => {
        this.loadTypes();
        this.closeEditModal();
        this.isSubmitting = false;
      },
      error: (error) => {
        console.error('Error updating type:', error);
        this.isSubmitting = false;
      }
    });
  }

  deleteType(id: string) {
    if (!confirm('Are you sure you want to delete this type?')) return;

    this.assetTypeService.delete(id).subscribe({
      next: () => {
        this.loadTypes();
      },
      error: (error) => {
        console.error('Error deleting type:', error);
      }
    });
  }

  getPropertyName(propertyId: string): string {
    const property = this.availableProperties.find(p => p.id === propertyId);
    return property?.name || propertyId;
  }
}
