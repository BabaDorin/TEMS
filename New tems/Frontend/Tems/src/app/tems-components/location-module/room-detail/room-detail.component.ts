import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
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
import { RoomWithHierarchy, RoomType, RoomStatus } from 'src/app/models/location/room.model';
import { Asset } from 'src/app/models/asset/asset.model';
import { AssetType } from 'src/app/models/asset/asset-type.model';
import { AssetDefinition } from 'src/app/models/asset/asset-definition.model';
import { AddAssetToRoomModalComponent } from '../add-asset-to-room-modal/add-asset-to-room-modal.component';

@Component({
  selector: 'app-room-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, AgGridAngular],
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
  assetsCurrentPage = 1;
  assetsPageSize = 50;
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
  isDropdownOpen = false;
  isDefinitionDropdownOpen = false;
  searchText = '';
  definitionSearchText = '';
  private assetTagSearchSubject = new Subject<string>();

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    if (!target.closest('.type-dropdown-container')) {
      this.isDropdownOpen = false;
    }
    if (!target.closest('.definition-dropdown-container')) {
      this.isDefinitionDropdownOpen = false;
    }
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
      minWidth: 120,
      cellClass: 'font-medium cursor-pointer',
      onCellClicked: (params) => {
        this.viewAssetDetails(params.data);
      },
      cellRenderer: (params: any) => {
        return `<span class="text-blue-600 hover:text-blue-800">${params.value}</span>`;
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
    }
  ];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private locationService: LocationService,
    private assetTypeService: AssetTypeService,
    private assetDefinitionService: AssetDefinitionService,
    private dialog: MatDialog
  ) {
    this.assetTagSearchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(() => {
      this.applyFilters();
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
      }
    });
  }

  loadAssetDefinitions() {
    this.assetDefinitionService.getAll().subscribe({
      next: (definitions) => {
        this.assetDefinitions = definitions;
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
    this.locationService.getAssetsByRoom(this.room.id, this.assetsCurrentPage, this.assetsPageSize).subscribe({
      next: (response) => {
        this.assets = response.data?.assets || response.assets || [];
        this.filteredAssets = [...this.assets];
        this.assetsTotalCount = response.data?.totalCount || response.totalCount || 0;
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
    this.applyFilters();
  }

  onAssetTagChange() {
    this.assetTagSearchSubject.next(this.assetTagSearch);
  }

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
    this.isDefinitionDropdownOpen = false;
  }

  toggleDefinitionDropdown() {
    if (this.selectedTypeIds.length === 0) return;
    this.isDefinitionDropdownOpen = !this.isDefinitionDropdownOpen;
    this.isDropdownOpen = false;
  }

  getFilteredAndSortedTypes(): AssetType[] {
    let filtered = this.assetTypes;
    if (this.searchText) {
      filtered = filtered.filter(t => t.name.toLowerCase().includes(this.searchText.toLowerCase()));
    }
    return filtered.sort((a, b) => a.name.localeCompare(b.name));
  }

  getFilteredAndSortedDefinitions(): AssetDefinition[] {
    let filtered = this.assetDefinitions.filter(d => 
      this.selectedTypeIds.includes(d.assetTypeId)
    );
    if (this.definitionSearchText) {
      filtered = filtered.filter(d => d.name.toLowerCase().includes(this.definitionSearchText.toLowerCase()));
    }
    return filtered.sort((a, b) => a.name.localeCompare(b.name));
  }

  toggleTypeSelection(typeId: string) {
    const index = this.selectedTypeIds.indexOf(typeId);
    if (index > -1) {
      this.selectedTypeIds.splice(index, 1);
      this.selectedDefinitionIds = this.selectedDefinitionIds.filter(defId => {
        const def = this.assetDefinitions.find(d => d.id === defId);
        return def && this.selectedTypeIds.includes(def.assetTypeId);
      });
    } else {
      this.selectedTypeIds.push(typeId);
    }
    this.applyFilters();
  }

  toggleDefinitionSelection(defId: string) {
    const index = this.selectedDefinitionIds.indexOf(defId);
    if (index > -1) {
      this.selectedDefinitionIds.splice(index, 1);
    } else {
      this.selectedDefinitionIds.push(defId);
    }
    this.applyFilters();
  }

  isTypeSelected(typeId: string): boolean {
    return this.selectedTypeIds.includes(typeId);
  }

  isDefinitionSelected(defId: string): boolean {
    return this.selectedDefinitionIds.includes(defId);
  }

  getTypeName(typeId: string): string {
    const type = this.assetTypes.find(t => t.id === typeId);
    return type?.name || typeId;
  }

  getDefinitionName(defId: string): string {
    const def = this.assetDefinitions.find(d => d.id === defId);
    return def?.name || defId;
  }

  getDefinitionTypeName(def: AssetDefinition): string {
    const type = this.assetTypes.find(t => t.id === def.assetTypeId);
    return type?.name || '';
  }

  removeTypeFilter(typeId: string) {
    this.toggleTypeSelection(typeId);
  }

  removeDefinitionFilter(defId: string) {
    this.toggleDefinitionSelection(defId);
  }

  applyFilters() {
    this.filteredAssets = this.assets.filter(asset => {
      if (this.assetTagSearch && !asset.assetTag.toLowerCase().includes(this.assetTagSearch.toLowerCase())) {
        return false;
      }
      if (this.selectedTypeIds.length > 0) {
        const typeId = asset.definition?.assetTypeId;
        if (!typeId || !this.selectedTypeIds.includes(typeId)) {
          return false;
        }
      }
      if (this.selectedDefinitionIds.length > 0) {
        const defId = asset.definition?.definitionId;
        if (!defId || !this.selectedDefinitionIds.includes(defId)) {
          return false;
        }
      }
      return true;
    });
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
      width: '500px',
      panelClass: 'custom-dialog',
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
}
