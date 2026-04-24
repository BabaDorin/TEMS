import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { trigger, transition, style, animate } from '@angular/animations';
import { MatDialog } from '@angular/material/dialog';
import { LocationService } from 'src/app/services/location.service';
import { ThemeService } from 'src/app/services/theme.service';
import { Site } from 'src/app/models/location/site.model';
import { Building } from 'src/app/models/location/building.model';
import { RoomWithHierarchy, RoomType, RoomStatus } from 'src/app/models/location/room.model';
import { RoomDetailModalComponent } from '../room-detail-modal/room-detail-modal.component';
import { AddRoomModalComponent } from '../add-room-modal/add-room-modal.component';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

@Component({
  selector: 'app-view-locations',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    AgGridAngular,
    CustomSelectComponent
  ],
  templateUrl: './view-locations.component.html',
  styleUrls: ['./view-locations.component.scss'],
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
export class ViewLocationsComponent implements OnInit, OnDestroy {
  rowData: RoomWithHierarchy[] = [];
  filteredRowData: RoomWithHierarchy[] = [];
  sites: Site[] = [];
  buildings: Building[] = [];
  gridApi!: GridApi;
  gridReady = false;

  // Filtering
  selectedSiteId: string | null = null;
  selectedBuildingId: string | null = null;
  roomNameSearch = '';
  isFiltersExpanded = false;
  private roomNameSearchSubject = new Subject<string>();

  // Select options
  siteOptions: SelectOption[] = [];
  buildingOptions: SelectOption[] = [];

  // Pagination
  currentPage = 1;
  paginationPageSize = 20;
  totalCount = 0;
  totalPages = 0;
  
  // Selection
  selectedRooms: RoomWithHierarchy[] = [];

  defaultColDef: ColDef = {
    sortable: true,
    filter: true,
    resizable: true,
    flex: 1
  };

  rowSelection: 'single' | 'multiple' = 'multiple';

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
      headerName: 'Room',
      field: 'name',
      flex: 1,
      minWidth: 220,
      cellClass: 'font-medium',
      onCellClicked: (params) => {
        const target = params.event?.target as HTMLElement;
        if (target?.closest('.room-identifier-link')) {
          this.router.navigate(['/locations', params.data.id]);
        }
      },
      cellRenderer: (params: any) => {
        const roomNumber = params.data.roomNumber ? ` (${params.data.roomNumber})` : '';
        return `
          <button class="room-identifier-link text-blue-600 hover:text-blue-800 hover:underline">${params.value}${roomNumber}</button>
        `;
      }
    },
    {
      headerName: 'Site',
      field: 'siteName',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => params.value || '—'
    },
    {
      headerName: 'Building',
      field: 'buildingName',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => params.value || '—'
    },
    {
      headerName: 'Floor',
      field: 'floorLabel',
      flex: 1,
      minWidth: 120,
      valueFormatter: (params) => params.value || '—'
    },
    {
      headerName: 'Type',
      field: 'type',
      flex: 1,
      minWidth: 120,
      cellRenderer: (params: any) => {
        const typeLabels: Record<RoomType, string> = {
          [RoomType.Meeting]: 'Meeting',
          [RoomType.Desk]: 'Desk',
          [RoomType.Workshop]: 'Workshop',
          [RoomType.ServerRoom]: 'Server Room'
        };
        return typeLabels[params.value as RoomType] || params.value;
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
          this.viewRoomDetails(params.data);
        }
      }
    }
  ];

  constructor(
    private router: Router,
    private locationService: LocationService,
    private dialog: MatDialog,
    private themeService: ThemeService
  ) {}

  get gridThemeClass(): string {
    return this.themeService.isDarkMode ? 'ag-theme-quartz-dark' : 'ag-theme-quartz';
  }

  ngOnInit() {
    this.loadSites();
    this.setupRoomNameSearch();
  }

  ngOnDestroy() {
    this.roomNameSearchSubject.complete();
  }

  loadSites() {
    this.locationService.getAllSites().subscribe({
      next: (sites) => {
        this.sites = sites.sort((a, b) => a.name.localeCompare(b.name));
        this.siteOptions = [{ value: '', label: 'All Sites' }, ...this.sites.map(s => ({ value: s.id, label: s.name }))];
        
        // Auto-select if only one site
        if (this.sites.length === 1) {
          this.selectedSiteId = this.sites[0].id;
          this.loadBuildings(this.selectedSiteId);
        } else {
          // Load all rooms without filtering
          this.loadRooms();
        }
      },
      error: (error) => {
        console.error('Error loading sites:', error);
      }
    });
  }

  loadBuildings(siteId?: string) {
    this.locationService.getAllBuildings(siteId).subscribe({
      next: (buildings) => {
        this.buildings = buildings.sort((a, b) => a.name.localeCompare(b.name));
        this.buildingOptions = [{ value: '', label: 'All Buildings' }, ...this.buildings.map(b => ({ value: b.id, label: b.name }))];
        
        // Auto-select if only one building
        if (this.selectedSiteId && this.buildings.length === 1) {
          this.selectedBuildingId = this.buildings[0].id;
        }
        
        // Load rooms after buildings are loaded
        this.loadRooms();
      },
      error: (error) => {
        console.error('Error loading buildings:', error);
      }
    });
  }

  loadRooms() {
    this.selectedRooms = [];
    this.gridApi?.deselectAll();

    this.locationService.getRoomsWithHierarchy(
      this.selectedSiteId || undefined,
      this.selectedBuildingId || undefined,
      this.currentPage,
      this.paginationPageSize,
      this.roomNameSearch.trim() || undefined
    ).subscribe({
      next: (response) => {
        const rooms = response.data || [];
        this.rowData = rooms.map(room => ({
          id: room.id,
          buildingId: room.buildingId,
          name: room.name,
          roomNumber: room.roomNumber,
          floorLabel: room.floorLabel,
          type: room.type as any,
          capacity: room.capacity,
          area: room.area,
          status: room.status as any,
          description: room.description,
          createdAt: new Date(room.createdAt),
          updatedAt: new Date(room.updatedAt),
          siteName: room.siteName,
          siteId: room.siteId,
          buildingName: room.buildingName,
          assetCounts: room.assetCounts
        }));
        this.filteredRowData = [...this.rowData];
        this.totalCount = response.totalCount || 0;
        this.totalPages = response.totalPages || Math.max(1, Math.ceil(this.totalCount / this.paginationPageSize));
        this.currentPage = response.pageNumber || this.currentPage;
      },
      error: (error) => {
        console.error('Error loading rooms:', error);
      }
    });
  }

  setupRoomNameSearch() {
    this.roomNameSearchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(() => {
        this.applyFilters();
      });
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
    this.gridReady = true;
  }

  onSelectionChanged() {
    if (this.gridApi) {
      this.selectedRooms = this.gridApi.getSelectedRows();
    }
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadRooms();
    }
  }

  getShowingStart(): number {
    if (this.totalCount === 0) return 0;
    return (this.currentPage - 1) * this.paginationPageSize + 1;
  }

  getShowingEnd(): number {
    return Math.min(this.currentPage * this.paginationPageSize, this.totalCount);
  }

  viewRoomDetails(room: RoomWithHierarchy) {
    const dialogRef = this.dialog.open(RoomDetailModalComponent, {
      width: '520px',
      maxWidth: '95vw',
      data: { room },
      panelClass: 'custom-dialog-container'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'view-full') {
        this.router.navigate(['/locations', room.id]);
      }
    });
  }

  openAddRoomModal() {
    const dialogRef = this.dialog.open(AddRoomModalComponent, {
      width: '90vw',
      maxWidth: '1200px',
      minHeight: '600px',
      maxHeight: '90vh',
      panelClass: 'custom-dialog-container',
      disableClose: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'saved') {
        // Reload rooms to show the newly created room
        this.loadRooms();
      } else if (result === 'open-full') {
        // Navigate to the full page view
        this.router.navigate(['/locations/new']);
      }
    });
  }

  toggleFilters() {
    this.isFiltersExpanded = !this.isFiltersExpanded;
  }

  onSiteChange() {
    // Reset building selection
    this.selectedBuildingId = null;
    this.currentPage = 1;
    
    // Load buildings for selected site (or all if none selected)
    this.loadBuildings(this.selectedSiteId || undefined);
  }

  onBuildingChange() {
    // Reload rooms when building selection changes
    this.currentPage = 1;
    this.loadRooms();
  }

  onRoomNameChange() {
    this.roomNameSearchSubject.next(this.roomNameSearch);
  }

  applyFilters() {
    this.currentPage = 1;
    this.loadRooms();
  }

  clearFilters() {
    this.selectedSiteId = null;
    this.selectedBuildingId = null;
    this.roomNameSearch = '';
    this.currentPage = 1;
    
    // Reload all data
    this.loadSites();
  }

  getActiveFilterCount(): number {
    let count = 0;
    if (this.selectedSiteId) count++;
    if (this.selectedBuildingId) count++;
    if (this.roomNameSearch.trim()) count++;
    return count;
  }

  selectSite(siteId: string) {
    this.selectedSiteId = siteId || null;
    this.onSiteChange();
  }

  selectBuilding(buildingId: string) {
    this.selectedBuildingId = buildingId || null;
    this.onBuildingChange();
  }
}
