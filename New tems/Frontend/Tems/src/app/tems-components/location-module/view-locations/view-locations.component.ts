import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
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
import { Site } from 'src/app/models/location/site.model';
import { Building } from 'src/app/models/location/building.model';
import { RoomWithHierarchy, RoomType, RoomStatus } from 'src/app/models/location/room.model';
import { RoomDetailModalComponent } from '../room-detail-modal/room-detail-modal.component';
import { AddRoomModalComponent } from '../add-room-modal/add-room-modal.component';

@Component({
  selector: 'app-view-locations',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    AgGridAngular
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

  // Dropdown states
  isSiteDropdownOpen = false;
  isBuildingDropdownOpen = false;
  siteSearchText = '';
  buildingSearchText = '';

  // Pagination
  currentPage = 1;
  paginationPageSize = 50;
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
      minWidth: 180,
      cellClass: 'font-medium cursor-pointer',
      onCellClicked: (params) => {
        this.viewRoomDetails(params.data);
      },
      cellRenderer: (params: any) => {
        const roomNumber = params.data.roomNumber ? ` (${params.data.roomNumber})` : '';
        return `<span class="text-blue-600 hover:text-blue-800">${params.value}${roomNumber}</span>`;
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
    }
  ];

  constructor(
    private router: Router,
    private locationService: LocationService,
    private dialog: MatDialog
  ) {}

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
    this.locationService.getRoomsWithHierarchy(
      this.selectedSiteId || undefined,
      this.selectedBuildingId || undefined
    ).subscribe({
      next: (rooms) => {
        this.rowData = rooms;
        this.applyFilters();
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
    this.updatePagination();
  }

  onSelectionChanged() {
    if (this.gridApi) {
      this.selectedRooms = this.gridApi.getSelectedRows();
    }
  }

  updatePagination() {
    this.totalCount = this.filteredRowData.length;
    this.totalPages = Math.ceil(this.totalCount / this.paginationPageSize);
    if (this.currentPage > this.totalPages && this.totalPages > 0) {
      this.currentPage = this.totalPages;
    }
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
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
      width: '500px',
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
    
    // Load buildings for selected site (or all if none selected)
    this.loadBuildings(this.selectedSiteId || undefined);
  }

  onBuildingChange() {
    // Reload rooms when building selection changes
    this.loadRooms();
  }

  onRoomNameChange() {
    this.roomNameSearchSubject.next(this.roomNameSearch);
  }

  applyFilters() {
    let filtered = [...this.rowData];

    // Room name filtering (client-side only)
    if (this.roomNameSearch.trim()) {
      const search = this.roomNameSearch.toLowerCase().trim();
      filtered = filtered.filter(room => 
        room.name.toLowerCase().includes(search) ||
        (room.roomNumber && room.roomNumber.toLowerCase().includes(search))
      );
    }

    this.filteredRowData = filtered;
    this.updatePagination();
  }

  clearFilters() {
    this.selectedSiteId = null;
    this.selectedBuildingId = null;
    this.roomNameSearch = '';
    
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

  getFilteredBuildings(): Building[] {
    // Buildings are already filtered by site when loaded
    if (!this.buildingSearchText.trim()) {
      return this.buildings;
    }
    const search = this.buildingSearchText.toLowerCase();
    return this.buildings.filter(b => b.name.toLowerCase().includes(search));
  }

  // Dropdown methods
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event) {
    const target = event.target as HTMLElement;
    if (!target.closest('.site-dropdown-container')) {
      this.isSiteDropdownOpen = false;
    }
    if (!target.closest('.building-dropdown-container')) {
      this.isBuildingDropdownOpen = false;
    }
  }

  toggleSiteDropdown() {
    this.isSiteDropdownOpen = !this.isSiteDropdownOpen;
    this.isBuildingDropdownOpen = false;
    if (this.isSiteDropdownOpen) {
      this.siteSearchText = '';
    }
  }

  toggleBuildingDropdown() {
    if (this.buildings.length === 0) return;
    this.isBuildingDropdownOpen = !this.isBuildingDropdownOpen;
    this.isSiteDropdownOpen = false;
    if (this.isBuildingDropdownOpen) {
      this.buildingSearchText = '';
    }
  }

  getFilteredSites(): Site[] {
    if (!this.siteSearchText.trim()) {
      return this.sites;
    }
    const search = this.siteSearchText.toLowerCase();
    return this.sites.filter(s => s.name.toLowerCase().includes(search));
  }

  selectSite(siteId: string | null) {
    this.selectedSiteId = siteId;
    this.isSiteDropdownOpen = false;
    this.siteSearchText = '';
    this.onSiteChange();
  }

  selectBuilding(buildingId: string | null) {
    this.selectedBuildingId = buildingId;
    this.isBuildingDropdownOpen = false;
    this.buildingSearchText = '';
    this.onBuildingChange();
  }

  getSiteName(siteId: string): string {
    const site = this.sites.find(s => s.id === siteId);
    return site ? site.name : '';
  }

  getBuildingName(buildingId: string): string {
    const building = this.buildings.find(b => b.id === buildingId);
    return building ? building.name : '';
  }
}
