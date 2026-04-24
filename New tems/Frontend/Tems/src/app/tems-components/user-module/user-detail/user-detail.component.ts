import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
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
  assetsTotalPages = 0;
  assetsCurrentPage = 1;
  assetsPageSize = 20;
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
      minWidth: 160,
      cellClass: 'font-medium',
      onCellClicked: (params) => {
        const target = params.event?.target as HTMLElement;
        if (target?.closest('.asset-identifier-link')) {
          this.navigateToAssetDetail(params.data.id);
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
      this.assetsCurrentPage = 1;
      this.loadAssets();
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
        this.definitionOptions = definitions.map(d => ({ value: d.name, label: d.name }));
        this.syncDefinitionOptions();
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
    this.selectedAssets = [];
    this.assetsGridApi?.deselectAll();
    this.userService.getUserAssets(
      this.user.id,
      this.assetsCurrentPage,
      this.assetsPageSize,
      this.assetTagSearch.trim() || undefined,
      this.selectedTypeIds.length > 0 ? this.selectedTypeIds : undefined,
      undefined,
      this.selectedDefinitionIds.length > 0 ? this.selectedDefinitionIds : undefined
    ).subscribe({
      next: (response) => {
        this.assets = response.assets || [];
        this.filteredAssets = [...this.assets];
        this.assetsTotalCount = response.totalCount || 0;
        this.assetsTotalPages = response.totalPages || Math.max(1, Math.ceil(this.assetsTotalCount / this.assetsPageSize));
        this.assetsCurrentPage = response.pageNumber || this.assetsCurrentPage;
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
    if (window.history.length > 1) {
      this.location.back();
      return;
    }

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
