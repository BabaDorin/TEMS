import { Component, OnInit, Inject, Optional, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { trigger, transition, style, animate } from '@angular/animations';
import { LocationService } from 'src/app/services/location.service';
import { Site } from 'src/app/models/location/site.model';
import { Building } from 'src/app/models/location/building.model';

@Component({
  selector: 'app-add-room-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-room-modal.component.html',
  styleUrls: ['./add-room-modal.component.scss'],
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
export class AddRoomModalComponent implements OnInit {
  sites: Site[] = [];
  buildings: Building[] = [];
  
  selectedSite: string | null = null;
  selectedBuilding: string | null = null;
  
  roomName = '';
  floor: number | null = null;
  roomType: string | null = null;
  roomNumber = '';
  capacity: number | null = null;
  description = '';
  isSaving = false;

  // UI State
  isSiteDropdownOpen = false;
  isBuildingDropdownOpen = false;
  isTypeDropdownOpen = false;

  roomTypes = [
    { value: 'Meeting', label: 'Meeting Room' },
    { value: 'Desk', label: 'Desk Area' },
    { value: 'Workshop', label: 'Workshop' },
    { value: 'ServerRoom', label: 'Server Room' }
  ];

  constructor(
    @Optional() public dialogRef: MatDialogRef<AddRoomModalComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any,
    private locationService: LocationService,
    private router: Router,
    private elementRef: ElementRef
  ) {}

  ngOnInit() {
    this.loadSites();
  }

  loadSites() {
    this.locationService.getAllSites().subscribe({
      next: (sites) => {
        this.sites = sites;
        // Auto-select if only one site
        if (this.sites.length === 1) {
          this.selectedSite = this.sites[0].id;
          this.loadBuildings();
        }
      },
      error: (error) => {
        console.error('Error loading sites:', error);
      }
    });
  }

  loadBuildings() {
    if (!this.selectedSite) return;
    
    this.locationService.getAllBuildings(this.selectedSite).subscribe({
      next: (buildings) => {
        this.buildings = buildings;
        // Auto-select if only one building
        if (this.buildings.length === 1) {
          this.selectedBuilding = this.buildings[0].id;
        }
      },
      error: (error) => {
        console.error('Error loading buildings:', error);
      }
    });
  }

  // Dropdown toggles
  toggleSiteDropdown() {
    this.isSiteDropdownOpen = !this.isSiteDropdownOpen;
    this.isBuildingDropdownOpen = false;
    this.isTypeDropdownOpen = false;
  }

  toggleBuildingDropdown() {
    this.isBuildingDropdownOpen = !this.isBuildingDropdownOpen;
    this.isSiteDropdownOpen = false;
    this.isTypeDropdownOpen = false;
  }

  toggleTypeDropdown() {
    this.isTypeDropdownOpen = !this.isTypeDropdownOpen;
    this.isSiteDropdownOpen = false;
    this.isBuildingDropdownOpen = false;
  }

  // Selection handlers
  selectSite(siteId: string) {
    this.selectedSite = siteId;
    this.selectedBuilding = null;
    this.buildings = [];
    this.isSiteDropdownOpen = false;
    this.loadBuildings();
  }

  selectBuilding(buildingId: string) {
    this.selectedBuilding = buildingId;
    this.isBuildingDropdownOpen = false;
  }

  selectRoomType(type: string) {
    this.roomType = type;
    this.isTypeDropdownOpen = false;
  }

  // Helper methods
  getSiteName(siteId: string | null): string {
    if (!siteId) return '';
    const site = this.sites.find(s => s.id === siteId);
    return site ? site.name : '';
  }

  getBuildingName(buildingId: string | null): string {
    if (!buildingId) return '';
    const building = this.buildings.find(b => b.id === buildingId);
    return building ? building.name : '';
  }

  getRoomTypeLabel(type: string | null): string {
    if (!type) return '';
    const roomType = this.roomTypes.find(t => t.value === type);
    return roomType ? roomType.label : '';
  }

  canSaveRoom(): boolean {
    return !!(
      this.selectedSite &&
      this.selectedBuilding &&
      this.roomName.trim() &&
      this.floor !== null &&
      this.roomType
    );
  }

  saveRoom() {
    if (!this.canSaveRoom() || this.isSaving) return;

    this.isSaving = true;

    const roomData = {
      buildingId: this.selectedBuilding,
      name: this.roomName,
      floorLabel: this.floor?.toString() || '0',
      type: this.roomType,
      status: 'Available',
      capacity: this.capacity || 0,
      roomNumber: this.roomNumber || undefined,
      description: this.description || undefined,
      createdBy: 'current-user'
    };

    this.locationService.createRoom(roomData).subscribe({
      next: (response) => {
        this.isSaving = false;
        if (this.dialogRef) {
          this.dialogRef.close('saved');
        } else {
          this.router.navigate(['/locations']);
        }
      },
      error: (error) => {
        console.error('Error creating room:', error);
        this.isSaving = false;
      }
    });
  }

  close() {
    if (this.dialogRef) {
      this.dialogRef.close();
    }
  }

  openInFullPage() {
    if (this.dialogRef) {
      this.dialogRef.close();
    }
    this.router.navigate(['/locations/add']);
  }

  // Close dropdowns when clicking outside
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event) {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isSiteDropdownOpen = false;
      this.isBuildingDropdownOpen = false;
      this.isTypeDropdownOpen = false;
    }
  }
}
