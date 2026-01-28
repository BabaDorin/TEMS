import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { AssetService, AssetPageResponse } from 'src/app/services/asset.service';
import { LocationService } from 'src/app/services/location.service';
import { Asset } from 'src/app/models/asset/asset.model';

@Component({
  selector: 'app-add-asset-to-room-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule],
  templateUrl: './add-asset-to-room-modal.component.html',
  styleUrls: ['./add-asset-to-room-modal.component.scss']
})
export class AddAssetToRoomModalComponent implements OnInit, OnDestroy {
  roomId: string;
  roomName: string;

  searchText = '';
  isDropdownOpen = false;
  searchResults: Asset[] = [];
  isSearching = false;
  selectedAsset: Asset | null = null;
  isSubmitting = false;

  private searchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(
    public dialogRef: MatDialogRef<AddAssetToRoomModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { roomId: string; roomName: string },
    private assetService: AssetService,
    private locationService: LocationService
  ) {
    this.roomId = data.roomId;
    this.roomName = data.roomName;
  }

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(searchText => {
      this.performSearch(searchText);
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSearchChange() {
    this.searchSubject.next(this.searchText);
    if (this.searchText.length > 0) {
      this.isDropdownOpen = true;
      this.isSearching = true;
    } else {
      this.isDropdownOpen = false;
      this.searchResults = [];
    }
  }

  onInputFocus() {
    if (this.searchText.length > 0 && this.searchResults.length > 0) {
      this.isDropdownOpen = true;
    }
  }

  performSearch(searchText: string) {
    if (!searchText || searchText.length < 1) {
      this.searchResults = [];
      this.isSearching = false;
      return;
    }

    this.isSearching = true;
    this.assetService.getAll(undefined, 1, 20, undefined, searchText).subscribe({
      next: (response: AssetPageResponse) => {
        this.searchResults = response.assets || [];
        this.isSearching = false;
      },
      error: () => {
        this.searchResults = [];
        this.isSearching = false;
      }
    });
  }

  selectAsset(asset: Asset) {
    this.selectedAsset = asset;
    this.searchText = asset.assetTag;
    this.isDropdownOpen = false;
    this.searchResults = [];
  }

  clearSelection() {
    this.selectedAsset = null;
    this.searchText = '';
    this.searchResults = [];
  }

  toggleDropdown() {
    if (this.searchText.length > 0 && this.searchResults.length > 0) {
      this.isDropdownOpen = !this.isDropdownOpen;
    }
  }

  close() {
    this.dialogRef.close();
  }

  addAssetToRoom() {
    if (!this.selectedAsset) return;

    this.isSubmitting = true;
    this.assetService.assignToRoom(this.selectedAsset.id, this.roomId).subscribe({
      next: () => {
        this.dialogRef.close({ success: true, asset: this.selectedAsset });
      },
      error: (error) => {
        console.error('Failed to add asset to room:', error);
        this.isSubmitting = false;
      }
    });
  }

  getAssetCurrentLocation(): string {
    if (!this.selectedAsset) return '—';
    const asset = this.selectedAsset as any;
    if (asset.locationId === this.roomId) {
      return 'Already in this room';
    }
    const loc = asset.locationDetails;
    if (loc?.fullPath) return loc.fullPath;
    if (loc?.name) return loc.name;
    const legacyLoc = this.selectedAsset.location;
    if (legacyLoc) {
      const parts = [];
      if (legacyLoc.building) parts.push(legacyLoc.building);
      if (legacyLoc.room) parts.push(legacyLoc.room);
      return parts.length > 0 ? parts.join(' > ') : 'Unassigned';
    }
    return 'Unassigned';
  }

  formatStatus(status: string): string {
    if (!status) return '—';
    return status.replace(/_/g, ' ').toLowerCase().replace(/\b\w/g, l => l.toUpperCase());
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return '—';
    return new Date(date).toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    });
  }
}
