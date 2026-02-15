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
import { UserService } from 'src/app/services/user.service';
import { AssetService } from 'src/app/services/asset.service';
import { AssetTypeService } from 'src/app/services/asset-type.service';
import { AssetDefinitionService } from 'src/app/services/asset-definition.service';
import { ThemeService } from 'src/app/services/theme.service';
import { UserDto, UserAssetDto } from 'src/app/models/user/user-management.model';
import { AssetType } from 'src/app/models/asset/asset-type.model';
import { AssetDefinition } from 'src/app/models/asset/asset-definition.model';
import { AllocateAssetModalComponent } from '../allocate-asset-modal/allocate-asset-modal.component';
import { DeleteUserConfirmModalComponent } from '../delete-user-confirm-modal/delete-user-confirm-modal.component';
import { EditUserRolesModalComponent } from '../../admin/user-management/edit-user-roles-modal/edit-user-roles-modal.component';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

@Component({
  selector: 'app-user-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, AgGridAngular, CustomSelectComponent],
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.scss'],
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
export class UserDetailComponent implements OnInit {
  user: UserDto | null = null;
  loading = true;
  error: string | null = null;
  activeTab: 'overview' | 'assets' | 'allocations' = 'overview';
  showActionsDropdown = false;

  // Assets Tab
  assets: UserAssetDto[] = [];
  filteredAssets: UserAssetDto[] = [];
  assetsLoading = false;
  assetsGridApi!: GridApi;
  assetsTotalCount = 0;
  assetsCurrentPage = 1;
  assetsPageSize = 50;
  selectedAssets: UserAssetDto[] = [];
  showPreviewModal = false;
  selectedAssetForPreview: UserAssetDto | null = null;
  isDefinitionExpanded = false;

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

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    if (!target.closest('.actions-dropdown')) {
      this.showActionsDropdown = false;
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
        if (status === 'AVAILABLE' || status === 'active') className = 'text-green-600';
        else if (status === 'IN_USE') className = 'text-blue-600';
        else if (status === 'UNDER_MAINTENANCE') className = 'text-yellow-600';
        else if (status === 'RETIRED') className = 'text-red-600';
        return `<span class="font-medium ${className}">${this.formatStatus(status)}</span>`;
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
        const parts = [];
        if (loc.building) parts.push(loc.building);
        if (loc.room) parts.push(loc.room);
        return parts.length > 0 ? parts.join(' > ') : '—';
      }
    },
    {
      headerName: 'Actions',
      field: 'id',
      flex: 1,
      minWidth: 100,
      sortable: false,
      filter: false,
      cellRenderer: (params: any) => {
        const container = document.createElement('div');
        container.className = 'flex gap-1 items-center h-full';
        
        const unassignBtn = document.createElement('button');
        unassignBtn.className = 'p-1 rounded hover:bg-gray-200 dark:hover:bg-gray-700 transition-colors text-red-600 dark:text-red-400';
        unassignBtn.innerHTML = '<svg width="16" height="16" viewBox="0 0 16 16" fill="none"><path d="M11 5L5 11M5 5L11 11" stroke="currentColor" stroke-width="1.5" stroke-linecap="round"/></svg>';
        unassignBtn.title = 'Unassign from user';
        unassignBtn.onclick = (e) => {
          e.stopPropagation();
          this.unassignAsset(params.data);
        };
        
        container.appendChild(unassignBtn);
        return container;
      }
    }
  ];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private assetService: AssetService,
    private assetTypeService: AssetTypeService,
    private assetDefinitionService: AssetDefinitionService,
    private dialog: MatDialog,
    private themeService: ThemeService
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
      this.loadUser(id);
      this.loadAssetTypes();
      this.loadAssetDefinitions();
    } else {
      this.error = 'No user ID provided';
      this.loading = false;
    }
  }

  loadAssetTypes() {
    this.assetTypeService.getAll().subscribe({
      next: (types) => {
        this.assetTypes = types;
        this.typeOptions = types.map(t => ({ value: t.id, label: t.name }));
      }
    });
  }

  loadAssetDefinitions() {
    this.assetDefinitionService.getAll().subscribe({
      next: (definitions) => {
        this.assetDefinitions = definitions;
        this.definitionOptions = definitions.map(d => ({ value: d.id, label: d.name }));
      }
    });
  }

  loadUser(id: string) {
    this.loading = true;
    this.error = null;
    
    this.userService.getUserById(id).subscribe({
      next: (user) => {
        this.user = user;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading user:', error);
        this.error = 'Failed to load user details';
        this.loading = false;
      }
    });
  }

  selectTab(tab: 'overview' | 'assets' | 'allocations') {
    this.activeTab = tab;
    if (tab === 'assets' && this.assets.length === 0 && this.user) {
      this.loadAssets();
    }
  }

  loadAssets() {
    if (!this.user) return;
    
    this.assetsLoading = true;
    this.userService.getUserAssets(this.user.id, this.assetsCurrentPage, this.assetsPageSize).subscribe({
      next: (response) => {
        this.assets = response.assets || [];
        this.filteredAssets = [...this.assets];
        this.assetsTotalCount = response.totalCount || 0;
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

  viewAssetDetails(asset: UserAssetDto) {
    this.selectedAssetForPreview = asset;
    this.showPreviewModal = true;
  }

  closePreviewModal() {
    this.showPreviewModal = false;
    this.selectedAssetForPreview = null;
    this.isDefinitionExpanded = false;
  }

  navigateToAssetDetail(assetId: string) {
    this.router.navigate(['/assets', assetId]);
  }

  formatStatus(status: string): string {
    if (!status) return '—';
    return status.replace(/_/g, ' ').toLowerCase().replace(/\b\w/g, l => l.toUpperCase());
  }

  goBack() {
    this.router.navigate(['/administration/users']);
  }

  editUserRoles() {
    if (!this.user) return;

    const dialogRef = this.dialog.open(EditUserRolesModalComponent, {
      width: '520px',
      maxWidth: '95vw',
      data: { user: this.user },
      panelClass: 'custom-dialog-container'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadUser(this.user!.id);
      }
    });
  }

  deleteUser() {
    if (!this.user) return;
    
    const dialogRef = this.dialog.open(DeleteUserConfirmModalComponent, {
      width: '520px',
      maxWidth: '95vw',
      panelClass: 'custom-dialog-container',
      data: { user: this.user }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.deleted) {
        this.router.navigate(['/admin/users']);
      }
    });
  }

  getUserFullName(): string {
    if (!this.user) return '';
    const parts = [];
    if (this.user.firstName) parts.push(this.user.firstName);
    if (this.user.lastName) parts.push(this.user.lastName);
    return parts.length > 0 ? parts.join(' ') : this.user.username;
  }

  formatDate(dateString: string): string {
    if (!dateString) return '—';
    return new Date(dateString).toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    });
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

  onTypeSelectionChange(ids: string[]) {
    this.selectedTypeIds = ids;
    this.selectedDefinitionIds = this.selectedDefinitionIds.filter(defId => {
      const def = this.assetDefinitions.find(d => d.id === defId);
      return def && this.selectedTypeIds.includes(def.assetTypeId);
    });
    this.definitionOptions = this.assetDefinitions
      .filter(d => this.selectedTypeIds.includes(d.assetTypeId))
      .map(d => ({ value: d.id, label: d.name }));
    this.applyFilters();
  }

  onDefinitionSelectionChange(ids: string[]) {
    this.selectedDefinitionIds = ids;
    this.applyFilters();
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
  openAllocateAssetModal() {
    if (!this.user) return;
    
    const dialogRef = this.dialog.open(AllocateAssetModalComponent, {
      width: '520px',
      maxWidth: '95vw',
      panelClass: 'custom-dialog-container',
      data: { 
        userId: this.user.id, 
        userName: this.getUserFullName(),
        allocationType: 'user'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.success) {
        this.loadAssets();
      }
    });
  }

  unassignAsset(asset: UserAssetDto) {
    if (!confirm(`Are you sure you want to unassign "${asset.assetTag}" from this user?`)) {
      return;
    }

    this.assetService.unassignFromUser(asset.id).subscribe({
      next: () => {
        this.loadAssets();
      },
      error: (error) => {
        console.error('Failed to unassign asset:', error);
        alert('Failed to unassign asset');
      }
    });
  }

  unassignSelected() {
    if (this.selectedAssets.length === 0) return;
    
    if (!confirm(`Are you sure you want to unassign ${this.selectedAssets.length} asset(s) from this user?`)) {
      return;
    }

    const promises = this.selectedAssets.map(asset => 
      this.assetService.unassignFromUser(asset.id).toPromise()
    );

    Promise.all(promises).then(() => {
      this.loadAssets();
      this.selectedAssets = [];
    }).catch(error => {
      console.error('Failed to unassign some assets:', error);
      this.loadAssets();
    });
  }
}
