import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { AssetService } from 'src/app/services/asset.service';
import { AssetTypeService } from 'src/app/services/asset-type.service';
import { AssetDefinitionService } from 'src/app/services/asset-definition.service';
import { Asset, AssetStatus } from 'src/app/models/asset/asset.model';
import { AssetType } from 'src/app/models/asset/asset-type.model';
import { AssetDefinition } from 'src/app/models/asset/asset-definition.model';
import { AssetLabelComponent } from '../../asset/asset-label/asset-label.component';

@Component({
  selector: 'app-view-assets',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    AgGridAngular,
    AssetLabelComponent
  ],
  templateUrl: './view-assets.component.html',
  styleUrls: ['./view-assets.component.scss']
})
export class ViewAssetsComponent implements OnInit {
  Object = Object;
  rowData: Asset[] = [];
  assetTypes: AssetType[] = [];
  filteredDefinitions: AssetDefinition[] = [];
  selectedDefinition: AssetDefinition | null = null;
  gridApi!: GridApi;
  showCreateModal = false;
  showPreviewModal = false;
  selectedAsset: Asset | null = null;
  createForm!: FormGroup;
  isSubmitting = false;
  gridReady = false;

  statusOptions = [
    { label: 'Available', value: AssetStatus.Available },
    { label: 'In Use', value: AssetStatus.InUse },
    { label: 'Under Maintenance', value: AssetStatus.UnderMaintenance },
    { label: 'Retired', value: AssetStatus.Retired }
  ];

  columnDefs: ColDef[] = [
    {
      headerName: 'Asset Tag',
      field: 'assetTag',
      flex: 1,
      minWidth: 120,
      cellClass: 'font-medium',
      onCellClicked: (params) => {
        this.viewAssetDetails(params.data);
      },
      cellRenderer: (params: any) => {
        return `<span class="text-blue-600 hover:text-blue-800 cursor-pointer">${params.value}</span>`;
      }
    },
    {
      headerName: 'Serial Number',
      field: 'serialNumber',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => params.value || '—'
    },
    {
      headerName: 'Type',
      field: 'assetTypeName',
      flex: 1,
      minWidth: 150
    },
    {
      headerName: 'Definition',
      field: 'definition.name',
      flex: 2,
      minWidth: 200
    },
    {
      headerName: 'Status',
      field: 'status',
      flex: 1,
      minWidth: 120,
      cellRenderer: (params: any) => {
        const status = params.value;
        let className = 'text-gray-600';
        if (status === 'AVAILABLE') className = 'text-green-600';
        else if (status === 'IN_USE') className = 'text-blue-600';
        else if (status === 'UNDER_MAINTENANCE') className = 'text-yellow-600';
        else if (status === 'RETIRED') className = 'text-red-600';
        return `<span class="${className}">${status}</span>`;
      }
    },
    {
      headerName: 'Location',
      field: 'location',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => {
        const loc = params.value;
        if (!loc) return '—';
        return [loc.building, loc.floor, loc.room].filter(v => v).join(' - ');
      }
    },
    {
      headerName: 'Assigned To',
      field: 'assignment.assignedToUserName',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => params.value || '—'
    }
  ];

  defaultColDef: ColDef = {
    sortable: true,
    filter: true,
    resizable: true,
    flex: 1
  };

  constructor(
    private assetService: AssetService,
    private assetTypeService: AssetTypeService,
    private assetDefinitionService: AssetDefinitionService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.createForm = this.fb.group({
      assetTypeId: ['', Validators.required],
      definitionId: ['', Validators.required],
      assetTag: ['', Validators.required],
      serialNumber: [''],
      status: [AssetStatus.Available, Validators.required],
      purchaseVendor: [''],
      purchaseDate: [''],
      purchasePrice: [''],
      warrantyExpiration: [''],
      invoiceNumber: [''],
      locationBuilding: [''],
      locationFloor: [''],
      locationRoom: [''],
      notes: ['']
    });
  }

  ngOnInit() {
    this.loadAssets();
    this.loadTypes();
    
    this.createForm.get('assetTypeId')?.valueChanges.subscribe(typeId => {
      if (typeId) {
        this.loadDefinitionsForType(typeId);
      } else {
        this.filteredDefinitions = [];
      }
      this.createForm.patchValue({ definitionId: '' });
      this.selectedDefinition = null;
    });

    this.createForm.get('definitionId')?.valueChanges.subscribe(defId => {
      if (defId) {
        const definition = this.filteredDefinitions.find(d => d.id === defId);
        this.selectedDefinition = definition || null;
      } else {
        this.selectedDefinition = null;
      }
    });
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
    this.gridReady = true;
    this.gridApi.sizeColumnsToFit();
  }

  loadAssets() {
    this.assetService.getAll().subscribe({
      next: (assets) => {
        this.rowData = assets;
      },
      error: (error) => {
        console.error('Error loading assets:', error);
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

  loadDefinitionsForType(typeId: string) {
    this.assetDefinitionService.getAll().subscribe({
      next: (definitions) => {
        this.filteredDefinitions = definitions.filter(d => d.assetTypeId === typeId && !d.isArchived);
      },
      error: (error) => {
        console.error('Error loading definitions:', error);
      }
    });
  }

  openCreateModal() {
    this.createForm.reset({ status: AssetStatus.Available });
    this.filteredDefinitions = [];
    this.selectedDefinition = null;
    this.showCreateModal = true;
  }

  closeCreateModal() {
    this.showCreateModal = false;
    this.createForm.reset();
  }

  createAsset() {
    if (this.createForm.invalid || this.isSubmitting) return;

    this.isSubmitting = true;
    const formValue = this.createForm.value;
    
    const request: any = {
      assetTypeId: formValue.assetTypeId,
      definitionId: formValue.definitionId,
      assetTag: formValue.assetTag,
      serialNumber: formValue.serialNumber || undefined,
      status: formValue.status
    };

    if (formValue.purchaseVendor || formValue.purchaseDate || formValue.purchasePrice) {
      request.purchaseInfo = {
        vendor: formValue.purchaseVendor || undefined,
        purchaseDate: formValue.purchaseDate || undefined,
        purchasePrice: formValue.purchasePrice ? parseFloat(formValue.purchasePrice) : undefined,
        warrantyExpiration: formValue.warrantyExpiration || undefined,
        invoiceNumber: formValue.invoiceNumber || undefined
      };
    }

    if (formValue.locationBuilding || formValue.locationFloor || formValue.locationRoom) {
      request.location = {
        building: formValue.locationBuilding || undefined,
        floor: formValue.locationFloor || undefined,
        room: formValue.locationRoom || undefined,
        updatedAt: new Date()
      };
    }

    if (formValue.notes) {
      request.notes = formValue.notes;
    }

    this.assetService.create(request).subscribe({
      next: () => {
        this.loadAssets();
        this.closeCreateModal();
        this.isSubmitting = false;
      },
      error: (error) => {
        console.error('Error creating asset:', error);
        this.isSubmitting = false;
      }
    });
  }

  viewAssetDetails(asset: Asset) {
    this.selectedAsset = asset;
    this.showPreviewModal = true;
  }

  closePreviewModal() {
    this.showPreviewModal = false;
    this.selectedAsset = null;
  }

  navigateToDetail(id: string) {
    this.router.navigate(['/assets', id]);
  }

  getLocationString(location: any): string {
    if (!location) return '—';
    return [location.building, location.floor, location.room].filter(v => v).join(' - ');
  }

  formatSpecValue(value: any, unit?: string): string {
    if (value === null || value === undefined) return '—';
    const stringValue = typeof value === 'boolean' ? (value ? 'Yes' : 'No') : String(value);
    return unit ? `${stringValue} ${unit}` : stringValue;
  }
}
