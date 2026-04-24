import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { MatDialog } from '@angular/material/dialog';
import { AssetService } from 'src/app/services/asset.service';
import { AssetTypeService } from 'src/app/services/asset-type.service';
import { AssetDefinitionService } from 'src/app/services/asset-definition.service';
import { ThemeService } from 'src/app/services/theme.service';
import { UserService } from 'src/app/services/user.service';
import { Asset, AssetStatus } from 'src/app/models/asset/asset.model';
import { AssetType } from 'src/app/models/asset/asset-type.model';
import { AssetDefinition } from 'src/app/models/asset/asset-definition.model';
import { AssetLabelComponent } from '../../asset/asset-label/asset-label.component';
import { AddAssetComponent } from '../../asset/add-asset/add-asset.component';
import { DownloadService } from 'src/app/download.service';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';
import { RoomDetailModalComponent } from '../../location-module/room-detail-modal/room-detail-modal.component';
import { LocationService } from 'src/app/services/location.service';
import { AssetBulkActionModalComponent } from './asset-bulk-action-modal.component';

@Component({
  selector: 'app-view-assets',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    AgGridAngular,
    AssetLabelComponent,
    CustomSelectComponent
  ],
  templateUrl: './view-assets.component.html',
  styleUrls: ['./view-assets.component.scss'],
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
export class ViewAssetsComponent implements OnInit, OnDestroy {
  Math = Math;
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

  // Filtering
  selectedTypeIds: string[] = [];
  selectedDefinitionIds: string[] = [];
  selectedDefinitionNames: string[] = [];
  typeOptions: SelectOption[] = [];
  definitionOptions: SelectOption[] = [];
  availableDefinitions: AssetDefinition[] = [];
  validUserIds = new Set<string>();
  isFiltersExpanded = false;
  assetTagSearch = '';
  private assetTagSearchSubject = new Subject<string>();
  isDefinitionExpanded = false;
  isPurchaseInfoExpanded = false;
  selectedAssigneeName = '';
  selectedAssigneeValid = false;

  @ViewChild('assetLabel') assetLabelComponent: AssetLabelComponent;

  // Pagination
  currentPage = 1;
  paginationPageSize = 20;
  totalCount = 0;
  totalPages = 0;
  
  // Selection
  selectedAssets: Asset[] = [];

  statusOptions = [
    { label: 'Available', value: AssetStatus.Available },
    { label: 'In Use', value: AssetStatus.InUse },
    { label: 'Under Maintenance', value: AssetStatus.UnderMaintenance },
    { label: 'Retired', value: AssetStatus.Retired }
  ];

  columnDefs: ColDef[] = [
    {
      headerCheckboxSelection: true,
      checkboxSelection: true,
      width: 50,
      maxWidth: 50,
      minWidth: 50,
      pinned: 'left',
      lockPosition: true,
      suppressMovable: true,
      resizable: false,
      sortable: false,
      filter: false,
      headerClass: 'ag-header-cell-center',
      cellClass: 'ag-cell-center'
    },
    {
      headerName: 'Asset Tag',
      field: 'assetTag',
      flex: 1,
      minWidth: 180,
      cellClass: 'font-medium',
      onCellClicked: (params) => {
        const target = params.event?.target as HTMLElement;
        if (target?.closest('.asset-identifier-link')) {
          this.navigateToDetail(params.data.id);
        }
      },
      cellRenderer: (params: any) => {
        const value = params.value || '—';
        return `
          <button class="asset-identifier-link text-blue-600 hover:text-blue-800 hover:underline">${value}</button>
        `;
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
      field: 'definition.assetTypeName',
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
      field: 'locationDetails',
      flex: 1,
      minWidth: 150,
      cellRenderer: (params: any) => {
        const asset = params.data;
        const locationDetails = asset?.locationDetails;
        const legacyLoc = asset?.location;
        
        let locationText = '—';
        let locationId = asset?.locationId;
        
        if (locationDetails?.fullPath) {
          locationText = locationDetails.fullPath;
        } else if (locationDetails?.name) {
          locationText = locationDetails.name;
        } else if (legacyLoc) {
          locationText = [legacyLoc.building, legacyLoc.floor, legacyLoc.room].filter(v => v).join(' - ') || '—';
          locationId = null;
        }
        
        if (locationId && locationText !== '—') {
          return `<a href="javascript:void(0)" class="text-gray-900 hover:text-[#007aff] hover:underline cursor-pointer" data-location-id="${locationId}">${locationText}</a>`;
        }
        return locationText;
      },
      onCellClicked: (params: any) => {
        const locationId = params.data?.locationId;
        if (locationId) {
          this.router.navigate(['/locations', locationId]);
        }
      }
    },
    {
      headerName: 'Assigned To',
      field: 'assignment.assignedToUserName',
      flex: 1,
      minWidth: 150,
      onCellClicked: (params: any) => {
        const target = params.event?.target as HTMLElement;
        const userId = params.data?.assignment?.assignedToUserId;
        if (target?.closest('.asset-assignee-link') && userId && this.validUserIds.has(userId)) {
          this.router.navigate(['/users', userId]);
        }
      },
      cellRenderer: (params: any) => {
        const userId = params.data?.assignment?.assignedToUserId;
        const userName = params.value || params.data?.assignment?.assignedToName || '—';

        if (!userId || !this.validUserIds.has(userId)) {
          return userName;
        }

        return `
          <button class="asset-assignee-link inline-flex items-center gap-1.5 text-gray-900 hover:text-[#007aff] hover:underline cursor-pointer">
            <svg width="13" height="13" viewBox="0 0 16 16" fill="none" class="text-gray-500">
              <path d="M13.333 14v-1.333c0-1.473-1.194-2.667-2.666-2.667H5.333c-1.473 0-2.666 1.194-2.666 2.667V14M8 7.333c-1.473 0-2.667-1.194-2.667-2.666C5.333 3.194 6.527 2 8 2c1.473 0 2.667 1.194 2.667 2.667C10.667 6.139 9.473 7.333 8 7.333Z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            <span>${userName}</span>
          </button>
        `;
      }
    },
    {
      headerName: 'Actions',
      field: 'id',
      flex: 0.7,
      minWidth: 100,
      sortable: false,
      filter: false,
      cellRenderer: () => {
        return `
          <div class="flex items-center justify-center h-full">
            <button class="action-view-btn text-blue-600 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300 cursor-pointer" title="Quick preview" aria-label="Quick preview">
              <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
                <path d="M1.333 8s2.667-4 6.667-4 6.667 4 6.667 4-2.667 4-6.667 4-6.667-4-6.667-4Z" stroke="currentColor" stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round"/>
                <circle cx="8" cy="8" r="2" stroke="currentColor" stroke-width="1.4"/>
              </svg>
            </button>
          </div>
        `;
      },
      onCellClicked: (params) => {
        const target = params.event?.target as HTMLElement;
        if (target?.closest('.action-view-btn')) {
          this.viewAssetDetails(params.data);
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

  rowSelection: 'single' | 'multiple' = 'multiple';

  constructor(
    private assetService: AssetService,
    private assetTypeService: AssetTypeService,
    private assetDefinitionService: AssetDefinitionService,
    private fb: FormBuilder,
    private router: Router,
    private dialog: MatDialog,
    private downloadService: DownloadService,
    private themeService: ThemeService,
    private userService: UserService,
    private locationService: LocationService
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

  get gridThemeClass(): string {
    return this.themeService.isDarkMode ? 'ag-theme-quartz-dark' : 'ag-theme-quartz';
  }

  ngOnInit() {
    this.loadAssets();
    this.loadTypes();
    this.loadValidUsers();
    
    // Setup debounced asset tag search
    this.assetTagSearchSubject.pipe(
      debounceTime(1000),
      distinctUntilChanged()
    ).subscribe(() => {
      this.currentPage = 1;
      this.loadAssets();
    });
    
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

  ngOnDestroy() {
    this.assetTagSearchSubject.complete();
  }

  toggleFilters() {
    this.isFiltersExpanded = !this.isFiltersExpanded;
  }

  getActiveFilterCount(): number {
    let count = 0;
    if (this.selectedTypeIds.length > 0) count++;
    if (this.selectedDefinitionIds.length > 0) count++;
    if (this.assetTagSearch && this.assetTagSearch.trim().length > 0) count++;
    return count;
  }

  onAssetTagChange() {
    this.assetTagSearchSubject.next(this.assetTagSearch);
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
    this.gridReady = true;
    this.gridApi.sizeColumnsToFit();
    
    // Listen to selection changes
    this.gridApi.addEventListener('selectionChanged', () => {
      this.selectedAssets = this.gridApi.getSelectedRows();
    });
  }

  loadAssets() {
    this.assetService.getAll(
      this.selectedTypeIds.length > 0 ? this.selectedTypeIds : undefined,
      this.currentPage,
      this.paginationPageSize,
      undefined,
      this.assetTagSearch && this.assetTagSearch.trim().length > 0 ? this.assetTagSearch.trim() : undefined,
      this.selectedDefinitionNames.length > 0 ? this.selectedDefinitionNames : undefined
    ).subscribe({
      next: (response) => {
        const filteredAssets = this.applyActiveFilters(response.assets);
        const clientFilterAdjusted = filteredAssets.length !== response.assets.length;

        this.rowData = filteredAssets;
        this.totalCount = clientFilterAdjusted ? filteredAssets.length : response.totalCount;
        this.totalPages = clientFilterAdjusted
          ? Math.max(1, Math.ceil(filteredAssets.length / this.paginationPageSize))
          : response.totalPages;
        this.currentPage = response.pageNumber;
        this.updateDefinitionOptionsFromAssets(filteredAssets);
        
        // Force AG-Grid to update
        if (this.gridApi) {
          this.gridApi.setGridOption('rowData', this.rowData);
        }
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
        this.typeOptions = this.assetTypes.map(t => ({ value: t.id, label: t.name }));
      },
      error: (error) => {
        console.error('Error loading types:', error);
      }
    });
  }

  loadDefinitionsForType(typeId: string) {
    this.assetDefinitionService.getAll().subscribe({
      next: (definitions) => {
        this.filteredDefinitions = definitions.filter(d => this.matchesAssetType(d, typeId) && !d.isArchived);
      },
      error: (error) => {
        console.error('Error loading definitions:', error);
      }
    });
  }

  openCreateModal() {
    const dialogRef = this.dialog.open(AddAssetComponent, {
      width: '90vw',
      maxWidth: '1200px',
      minHeight: '600px',
      maxHeight: '90vh',
      panelClass: 'custom-dialog-container',
      disableClose: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Refresh the asset list after successful creation
        this.loadAssets();
      }
    });
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
    this.resolveSelectedAssignee();
  }

  closePreviewModal() {
    this.showPreviewModal = false;
    this.selectedAsset = null;
  }

  downloadAssetLabel(event: Event) {
    event.stopPropagation();
    if (!this.selectedAsset || !this.assetLabelComponent) return;
    
    // Give the component a moment to render if needed
    setTimeout(() => {
      this.assetLabelComponent.downloadLabel();
    }, 100);
  }

  navigateToDetail(id: string) {
    this.router.navigate(['/assets', id]);
  }

  formatSpecValue(value: any, unit?: string): string {
    if (value === null || value === undefined) return '—';
    const stringValue = typeof value === 'boolean' ? (value ? 'Yes' : 'No') : String(value);
    return unit ? `${stringValue} ${unit}` : stringValue;
  }



  loadDefinitionsForSelectedTypes() {
    if (this.selectedTypeIds.length === 0) {
      this.availableDefinitions = [];
      this.selectedDefinitionIds = [];
      this.selectedDefinitionNames = [];
      this.definitionOptions = [];
      return;
    }

    this.assetDefinitionService.getAll(true).subscribe({
      next: (definitions) => {
        this.availableDefinitions = definitions.filter(d => 
          this.selectedTypeIds.some((typeId) => this.matchesAssetType(d, typeId))
        );

        this.assetService.getAll(this.selectedTypeIds, 1, 500).subscribe({
          next: (response) => {
            const catalogOptions = this.availableDefinitions.map(d => ({ value: d.name, label: d.name }));
            this.definitionOptions = this.mergeDefinitionOptions(
              catalogOptions,
              this.getDefinitionOptionsFromAssets(response.assets)
            );
            this.selectedDefinitionIds = this.selectedDefinitionIds.filter((name) =>
              this.definitionOptions.some((option) => option.value === name)
            );
            this.selectedDefinitionNames = [...this.selectedDefinitionIds];
          },
          error: () => {
            this.definitionOptions = this.availableDefinitions.map(d => ({ value: d.name, label: d.name }));
            this.selectedDefinitionNames = [...this.selectedDefinitionIds];
          }
        });
      },
      error: (error) => {
        console.error('Error loading definitions:', error);
      }
    });
  }

  onTypeSelectionChange(ids: string[]) {
    this.selectedTypeIds = ids;
    this.loadDefinitionsForSelectedTypes();
    this.currentPage = 1;
    this.loadAssets();
  }

  onDefinitionSelectionChange(ids: string[]) {
    this.selectedDefinitionIds = ids;
    this.selectedDefinitionNames = [...ids];
    this.currentPage = 1;
    this.loadAssets();
  }

  clearFilters() {
    this.selectedTypeIds = [];
    this.selectedDefinitionIds = [];
    this.selectedDefinitionNames = [];
    this.availableDefinitions = [];
    this.definitionOptions = [];
    this.assetTagSearch = '';
    this.currentPage = 1;
    this.loadAssets();
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadAssets();
  }

  assignToUser() {
    if (this.selectedAssets.length === 0) return;

    const dialogRef = this.dialog.open(AssetBulkActionModalComponent, {
      width: '520px',
      maxWidth: '95vw',
      panelClass: 'custom-dialog-container',
      data: {
        mode: 'user',
        assets: this.selectedAssets
      }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result?.success) {
        this.loadAssets();
        this.selectedAssets = [];
      }
    });
  }

  moveToRoom() {
    if (this.selectedAssets.length === 0) return;

    const dialogRef = this.dialog.open(AssetBulkActionModalComponent, {
      width: '520px',
      maxWidth: '95vw',
      panelClass: 'custom-dialog-container',
      data: {
        mode: 'room',
        assets: this.selectedAssets
      }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result?.success) {
        this.loadAssets();
        this.selectedAssets = [];
      }
    });
  }

  getLocationString(location: any, asset?: Asset): string {
    const assetData = asset || this.selectedAsset;
    if (assetData) {
      const a = assetData as any;
      if (a.locationDetails?.fullPath) {
        return a.locationDetails.fullPath;
      }
      if (a.locationDetails?.name) {
        return a.locationDetails.name;
      }
    }
    if (!location) return '—';
    return [location.building, location.floor, location.room].filter(v => v).join(' - ') || '—';
  }

  navigateToLocation(asset?: Asset) {
    const assetData = asset || this.selectedAsset;
    const a = assetData as any;
    if (a?.locationId) {
      this.router.navigate(['/locations', a.locationId]);
    }
  }

  hasLocationId(asset?: Asset): boolean {
    const assetData = asset || this.selectedAsset;
    return !!(assetData as any)?.locationId;
  }

  navigateToAssignee() {
    if (!this.selectedAsset?.assignment?.assignedToUserId || !this.selectedAssigneeValid) return;
    this.router.navigate(['/users', this.selectedAsset.assignment.assignedToUserId]);
  }

  openLocationModal() {
    if (!this.selectedAsset?.locationId) return;
    this.locationService.getRoomById(this.selectedAsset.locationId).subscribe({
      next: (room) => {
        this.dialog.open(RoomDetailModalComponent, {
          width: '520px',
          maxWidth: '95vw',
          data: { room },
          panelClass: 'custom-dialog-container'
        });
      }
    });
  }

  getAssigneeName(): string {
    return this.selectedAssigneeName;
  }

  private resolveSelectedAssignee() {
    this.selectedAssigneeName = '';
    this.selectedAssigneeValid = false;

    const assignedUserId = this.selectedAsset?.assignment?.assignedToUserId;
    if (!assignedUserId) return;

    this.userService.getUserById(assignedUserId).subscribe({
      next: (user) => {
        const fullName = [user.firstName, user.lastName].filter(Boolean).join(' ').trim();
        this.selectedAssigneeName = fullName || user.username || user.email || this.selectedAsset?.assignment?.assignedToUserName || '';
        this.selectedAssigneeValid = this.selectedAssigneeName.trim().length > 0;
      },
      error: () => {
        this.selectedAssigneeName = '';
        this.selectedAssigneeValid = false;
      }
    });
  }

  private loadValidUsers() {
    this.userService.getAllUsers(1, 500).subscribe({
      next: (response) => {
        this.validUserIds = new Set((response.users ?? []).map((user) => user.id));
        this.gridApi?.refreshCells({ force: true });
      },
      error: () => {
        this.validUserIds.clear();
        this.gridApi?.refreshCells({ force: true });
      }
    });
  }

  private matchesAssetType(definition: AssetDefinition, typeId: string): boolean {
    const def = definition as any;
    return def.assetTypeId === typeId || def.asset_type_id === typeId || def.assetType?.id === typeId;
  }

  private getDefinitionOptionsFromAssets(assets: Asset[]): SelectOption[] {
    const options = new Map<string, SelectOption>();

    assets.forEach((asset) => {
      const definitionName = asset.definition?.name?.trim();
      const typeId = asset.definition?.assetTypeId;

      if (!definitionName || !typeId || !this.selectedTypeIds.includes(typeId)) {
        return;
      }

      options.set(definitionName, { value: definitionName, label: definitionName });
    });

    return Array.from(options.values()).sort((left, right) => left.label.localeCompare(right.label));
  }

  private mergeDefinitionOptions(...groups: SelectOption[][]): SelectOption[] {
    const merged = new Map<string, SelectOption>();

    groups.flat().forEach((option) => {
      merged.set(option.value, option);
    });

    return Array.from(merged.values()).sort((left, right) => left.label.localeCompare(right.label));
  }

  private updateDefinitionOptionsFromAssets(assets: Asset[]) {
    if (this.selectedTypeIds.length === 0) {
      return;
    }

    this.definitionOptions = this.mergeDefinitionOptions(
      this.definitionOptions,
      this.getDefinitionOptionsFromAssets(assets)
    );
  }

  private applyActiveFilters(assets: Asset[]): Asset[] {
    return assets.filter((asset) => {
      if (this.selectedTypeIds.length > 0) {
        const assetTypeId = asset.definition?.assetTypeId?.trim();
        if (!assetTypeId || !this.selectedTypeIds.includes(assetTypeId)) {
          return false;
        }
      }

      if (this.selectedDefinitionNames.length > 0) {
        const definitionName = asset.definition?.name?.trim().toLowerCase();
        const selectedNames = this.selectedDefinitionNames.map((name) => name.trim().toLowerCase());
        if (!definitionName || !selectedNames.includes(definitionName)) {
          return false;
        }
      }

      if (this.assetTagSearch.trim().length > 0) {
        const assetTag = asset.assetTag?.toLowerCase() ?? '';
        if (!assetTag.includes(this.assetTagSearch.trim().toLowerCase())) {
          return false;
        }
      }

      return true;
    });
  }
}
