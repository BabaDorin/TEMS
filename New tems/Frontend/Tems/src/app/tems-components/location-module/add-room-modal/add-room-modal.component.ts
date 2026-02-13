import { Component, OnInit, Inject, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { trigger, transition, style, animate } from '@angular/animations';
import { LocationService } from 'src/app/services/location.service';
import { Site } from 'src/app/models/location/site.model';
import { Building } from 'src/app/models/location/building.model';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

@Component({
  selector: 'app-add-room-modal',
  standalone: true,
    imports: [CommonModule, FormsModule, CustomSelectComponent],
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
  
    selectedSite: string = '';
    selectedBuilding: string = '';
    
    roomName = '';
    floor: number | null = null;
    roomType: string = '';
    roomNumber = '';
    capacity: number | null = null;
    description = '';
    isSaving = false;

    siteOptions: SelectOption[] = [];
    buildingOptions: SelectOption[] = [];
    roomTypeOptions: SelectOption[] = [
      { value: 'Meeting', label: 'Meeting Room' },
      { value: 'Desk', label: 'Desk Area' },
      { value: 'Workshop', label: 'Workshop' },
      { value: 'ServerRoom', label: 'Server Room' }
    ];

    constructor(
      @Optional() public dialogRef: MatDialogRef<AddRoomModalComponent>,
      @Optional() @Inject(MAT_DIALOG_DATA) public data: any,
      private locationService: LocationService,
      private router: Router
    ) {}

  ngOnInit() {
    this.loadSites();
  }

  loadSites() {
    this.locationService.getAllSites().subscribe({
      next: (sites) => {
        this.sites = sites;
        this.siteOptions = sites.map(s => ({ value: s.id, label: s.name }));
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
        this.buildingOptions = buildings.map(b => ({ value: b.id, label: b.name }));
        if (this.buildings.length === 1) {
          this.selectedBuilding = this.buildings[0].id;
        }
      },
      error: (error) => {
        console.error('Error loading buildings:', error);
      }
    });
  }

  onSiteChange(siteId: string) {
    this.selectedSite = siteId;
    this.selectedBuilding = '';
    this.buildings = [];
    this.buildingOptions = [];
    if (siteId) {
      this.loadBuildings();
    }
  }

  onBuildingChange(buildingId: string) {
    this.selectedBuilding = buildingId;
  }

  onRoomTypeChange(type: string) {
    this.roomType = type;
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
}
