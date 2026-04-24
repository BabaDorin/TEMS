import { Component, OnInit } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { trigger, transition, style, animate } from '@angular/animations';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { MatDialog } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { LocationService } from 'src/app/services/location.service';
import { AssetTypeService } from 'src/app/services/asset-type.service';
import { AssetDefinitionService } from 'src/app/services/asset-definition.service';
import { ThemeService } from 'src/app/services/theme.service';
import { RoomWithHierarchy, RoomType, RoomStatus } from 'src/app/models/location/room.model';
import { Asset } from 'src/app/models/asset/asset.model';
import { AssetType } from 'src/app/models/asset/asset-type.model';
import { AssetDefinition } from 'src/app/models/asset/asset-definition.model';
import { AddAssetToRoomModalComponent } from '../add-asset-to-room-modal/add-asset-to-room-modal.component';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

@Component({
  selector: 'app-room-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, AgGridAngular, CustomSelectComponent],
  templateUrl: './room-detail.component.html',
  styleUrls: ['./room-detail.component.scss'],
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
export class RoomDetailComponent implements OnInit {
  room: RoomWithHierarchy | null = null;
  loading = true;
  error: string | null = null;
  activeTab: 'overview' | 'assets' | 'allocations' = 'overview';
  showActionsDropdown = false;

  // Assets Tab
  assets: Asset[] = [];
  filteredAssets: Asset[] = [];
  assetsLoading = false;
  assetsGridApi!: GridApi;
  assetsTotalCount = 0;
  assetsTotalPages = 0;
  assetsCurrentPage = 1;
  assetsPageSize = 20;
  selectedAssets: Asset[] = [];
  showPreviewModal = false;
  selectedAssetForPreview: Asset | null = null;
  isDefinitionExpanded = false;
  isPurchaseInfoExpanded = false;

  // Filters
  isFiltersExpanded = false;
  assetTagSearch = '';
  selectedTypeIds: string[] = [];
  selectedDefinitionIds: string[] = [];
  assetTypes: AssetType[] = [];
  assetDefinitions: AssetDefinition[] = [];
  typeOptions: SelectOption[] = [];
  definitionOptions: SelectOption[] = [];
  private assetTagSearchSubject = new Subject<string>();

  get gridThemeClass(): string {
    return this.themeService.isDarkMode ? 'ag-theme-quartz-dark' : 'ag-theme-quartz';
  }

  assetsColumnDefs: ColDef[] = [
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
      filter: false
    },
    {
      headerName: 'Asset Tag',
      field: 'assetTag',
      flex: 1,
      minWidth: 160,
      cellClass: 'font-medium',
      onCellClicked: (params) => {
        const target = params.event?.target as HTMLElement;
        if (target?.closest('.asset-identifier-link')) {
          this.navigateToDetail(params.data.id);
        }
      },
      cellRenderer: (params: any) => {
        return `
          <button class="asset-identifier-link text-blue-600 hover:text-blue-800 hover:underline">${params.value}</button>
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
        return `<span class="font-medium ${className}">${this.formatStatus(status)}</span>`;
      }
    },
    {
      headerName: 'Assigned To',
      field: 'assignment.assignedToName',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => params.value || '—'
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

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    private locationService: LocationService,
    private assetTypeService: AssetTypeService,
    private assetDefinitionService: AssetDefinitionService,
    private dialog: MatDialog,
    private themeService: ThemeService
  ) {
    this.assetTagSearchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(() => {
      this.assetsCurrentPage = 1;
      this.loadAssets();
    });
  }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadRoom(id);
      this.loadAssetTypes();
      this.loadAssetDefinitions();
    } else {
      this.error = 'No room ID provided';
      this.loading = false;
    }
  }

  loadAssetTypes() {
    this.assetTypeService.getAll().subscribe({
      next: (types) => {
        this.assetTypes = types;
        this.typeOptions = this.assetTypes.map(t => ({ value: t.id, label: t.name }));
      }
    });
  }

  loadAssetDefinitions() {
    this.assetDefinitionService.getAll().subscribe({
      next: (definitions) => {
        this.assetDefinitions = definitions;
        this.definitionOptions = this.assetDefinitions.map(d => ({ value: d.name, label: d.name }));
        this.syncDefinitionOptions();
      }
    });
  }

  loadRoom(id: string) {
    this.loading = true;
    this.error = null;
    
    this.locationService.getRoomById(id).subscribe({
      next: (room) => {
        this.room = room;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading room:', error);
        this.error = 'Failed to load room details';
        this.loading = false;
      }
    });
  }

  selectTab(tab: 'overview' | 'assets' | 'allocations') {
    this.activeTab = tab;
    if (tab === 'assets' && this.assets.length === 0 && this.room) {
      this.loadAssets();
    }
  }

  loadAssets() {
    if (!this.room) return;
    
    this.assetsLoading = true;
    this.selectedAssets = [];
    this.assetsGridApi?.deselectAll();
    this.locationService.getAssetsByRoom(
      this.room.id,
      this.assetsCurrentPage,
      this.assetsPageSize,
      this.assetTagSearch.trim() || undefined,
      this.selectedTypeIds.length > 0 ? this.selectedTypeIds : undefined,
      undefined,
      this.selectedDefinitionIds.length > 0 ? this.selectedDefinitionIds : undefined
    ).subscribe({
      next: (response) => {
        this.assets = response.data?.assets || response.assets || [];
        this.filteredAssets = [...this.assets];
        this.assetsTotalCount = response.data?.totalCount || response.totalCount || 0;
        this.assetsTotalPages = response.data?.totalPages || response.totalPages || Math.max(1, Math.ceil(this.assetsTotalCount / this.assetsPageSize));
        this.assetsCurrentPage = response.data?.pageNumber || response.pageNumber || this.assetsCurrentPage;
        this.syncDefinitionOptions();
        this.assetsLoading = false;
      },
      error: (error) => {
        console.error('Error loading assets:', error);
        this.assetsLoading = false;
      }
    });
  }

  onAssetsGridReady(params: GridReadyEvent) {
    this.assetsGridApi = params.api;
  }

  viewAssetDetails(asset: Asset) {
    this.selectedAssetForPreview = asset;
    this.showPreviewModal = true;
  }

  closePreviewModal() {
    this.showPreviewModal = false;
    this.selectedAssetForPreview = null;
    this.isDefinitionExpanded = false;
    this.isPurchaseInfoExpanded = false;
  }

  navigateToDetail(assetId: string) {
    this.router.navigate(['/assets', assetId]);
  }

  downloadAssetLabel(event: Event) {
    event.stopPropagation();
    console.log('Download asset label for:', this.selectedAssetForPreview?.assetTag);
  }

  formatSpecValue(value: any, unit?: string): string {
    if (value === null || value === undefined) return '—';
    return unit ? `${value} ${unit}` : value.toString();
  }

  formatStatus(status: string): string {
    return status.replace(/_/g, ' ').toLowerCase().replace(/\b\w/g, l => l.toUpperCase());
  }

  goBack() {
    if (window.history.length > 1) {
      this.location.back();
      return;
    }

    this.router.navigate(['/locations']);
  }

  editRoom() {
    console.log('Edit room:', this.room?.id);
  }

  deleteRoom() {
    if (!this.room) return;
    
    if (confirm(`Are you sure you want to delete room ${this.room.name}?`)) {
      // Implement delete functionality
      console.log('Delete room:', this.room.id);
    }
  }

  getStatusClass(status: RoomStatus): string {
    switch (status) {
      case RoomStatus.Available:
        return 'bg-green-100 text-green-700';
      case RoomStatus.Maintenance:
        return 'bg-yellow-100 text-yellow-700';
      case RoomStatus.Decommissioned:
        return 'bg-red-100 text-red-700';
      default:
        return 'bg-gray-100 text-gray-700';
    }
  }

  getStatusLabel(status: RoomStatus): string {
    switch (status) {
      case RoomStatus.Available:
        return 'Available';
      case RoomStatus.Maintenance:
        return 'Maintenance';
      case RoomStatus.Decommissioned:
        return 'Decommissioned';
      default:
        return 'Unknown';
    }
  }

  getTypeLabel(type: RoomType): string {
    switch (type) {
      case RoomType.Meeting:
        return 'Meeting Room';
      case RoomType.Desk:
        return 'Desk Area';
      case RoomType.Workshop:
        return 'Workshop';
      case RoomType.ServerRoom:
        return 'Server Room';
      default:
        return 'Unknown';
    }
  }

  getHierarchyPath(): string {
    if (!this.room) return '';
    const parts = [];
    if (this.room.siteName) parts.push(this.room.siteName);
    if (this.room.buildingName) parts.push(this.room.buildingName);
    if (this.room.floorLabel) {
      const floorLabel = this.room.floorLabel.toLowerCase().includes('floor') 
        ? this.room.floorLabel 
        : `Floor ${this.room.floorLabel}`;
      parts.push(floorLabel);
    }
    return parts.join(' → ');
  }

  // Filter Methods
  toggleFilters() {
    this.isFiltersExpanded = !this.isFiltersExpanded;
  }

  getActiveFilterCount(): number {
    let count = 0;
    if (this.assetTagSearch) count++;
    if (this.selectedTypeIds.length > 0) count++;
    if (this.selectedDefinitionIds.length > 0) count++;
    return count;
  }

  clearFilters() {
    this.assetTagSearch = '';
    this.selectedTypeIds = [];
    this.selectedDefinitionIds = [];
    this.syncDefinitionOptions();
    this.assetsCurrentPage = 1;
    this.loadAssets();
  }

  onAssetTagChange() {
    this.assetTagSearchSubject.next(this.assetTagSearch);
  }

  onTypeSelectionChange(ids: string[]) {
    this.selectedTypeIds = ids;
    this.selectedDefinitionIds = this.selectedDefinitionIds.filter(definitionName =>
      this.assetDefinitions.some(definition =>
        definition.name === definitionName &&
        this.selectedTypeIds.some(typeId => this.matchesAssetType(definition, typeId))
      )
    );
    this.syncDefinitionOptions();
    this.applyFilters();
  }

  onDefinitionSelectionChange(ids: string[]) {
    this.selectedDefinitionIds = ids;
    this.applyFilters();
  }

  applyFilters() {
    this.assetsCurrentPage = 1;
    this.loadAssets();
  }

  onSelectionChanged() {
    if (this.assetsGridApi) {
      this.selectedAssets = this.assetsGridApi.getSelectedRows();
    }
  }

  // Action Methods
  openAddAssetModal() {
    if (!this.room) return;
    
    const dialogRef = this.dialog.open(AddAssetToRoomModalComponent, {
      width: '520px',
      maxWidth: '95vw',
      panelClass: 'custom-dialog-container',
      data: { roomId: this.room.id, roomName: this.room.name }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.success) {
        this.loadAssets();
      }
    });
  }

  moveToRoom() {
    // Disabled for now
  }

  archiveAssets() {
    // Disabled for now
  }

  private syncDefinitionOptions() {
    const filtered = this.assetDefinitions.filter((definition) => {
      if (this.selectedTypeIds.length === 0) {
        return true;
      }

      return this.selectedTypeIds.some((typeId) => this.matchesAssetType(definition, typeId));
    });

    this.definitionOptions = filtered
      .map((definition) => ({ value: definition.name, label: definition.name }))
      .sort((left, right) => left.label.localeCompare(right.label));
  }

  goToPage(page: number) {
    if (page < 1 || page > this.assetsTotalPages) return;
    this.assetsCurrentPage = page;
    this.loadAssets();
  }

  getAssetsShowingStart(): number {
    if (this.assetsTotalCount === 0) return 0;
    return (this.assetsCurrentPage - 1) * this.assetsPageSize + 1;
  }

  getAssetsShowingEnd(): number {
    return Math.min(this.assetsCurrentPage * this.assetsPageSize, this.assetsTotalCount);
  }

  private matchesAssetType(definition: AssetDefinition, typeId: string): boolean {
    const def = definition as any;
    return def.assetTypeId === typeId || def.asset_type_id === typeId || def.assetType?.id === typeId;
  }
}
