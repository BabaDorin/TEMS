import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { AssetService, AssetPageResponse } from 'src/app/services/asset.service';
import { Asset } from 'src/app/models/asset/asset.model';

export interface AllocateAssetModalData {
  allocationType: 'room' | 'user';
  roomId?: string;
  roomName?: string;
  userId?: string;
  userName?: string;
}

@Component({
  selector: 'app-allocate-asset-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule],
  templateUrl: './allocate-asset-modal.component.html',
  styleUrls: ['./allocate-asset-modal.component.scss']
})
export class AllocateAssetModalComponent implements OnInit, OnDestroy {
  allocationType: 'room' | 'user';
  targetId: string;
  targetName: string;

  searchText = '';
  isDropdownOpen = false;
  searchResults: Asset[] = [];
  isSearching = false;
  selectedAsset: Asset | null = null;
  isSubmitting = false;

  private searchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(
    public dialogRef: MatDialogRef<AllocateAssetModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AllocateAssetModalData,
    private assetService: AssetService
  ) {
    this.allocationType = data.allocationType;
    if (data.allocationType === 'room') {
      this.targetId = data.roomId || '';
      this.targetName = data.roomName || '';
    } else {
      this.targetId = data.userId || '';
      this.targetName = data.userName || '';
    }
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

  get headerTitle(): string {
    return this.allocationType === 'room' ? 'Add Asset to Room' : 'Allocate Asset to User';
  }

  get buttonText(): string {
    if (this.isSubmitting) {
      return this.allocationType === 'room' ? 'Adding...' : 'Allocating...';
    }
    return this.allocationType === 'room' ? 'Add to Room' : 'Allocate to User';
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

  allocateAsset() {
    if (!this.selectedAsset) return;

    this.isSubmitting = true;

    if (this.allocationType === 'room') {
      this.assetService.assignToRoom(this.selectedAsset.id, this.targetId).subscribe({
        next: () => {
          this.dialogRef.close({ success: true, asset: this.selectedAsset });
        },
        error: (error) => {
          console.error('Failed to add asset to room:', error);
          this.isSubmitting = false;
        }
      });
    } else {
      this.assetService.assignToUser(this.selectedAsset.id, this.targetId, this.targetName).subscribe({
        next: () => {
          this.dialogRef.close({ success: true, asset: this.selectedAsset });
        },
        error: (error) => {
          console.error('Failed to allocate asset to user:', error);
          this.isSubmitting = false;
        }
      });
    }
  }

  getAssetCurrentAssignment(): string {
    if (!this.selectedAsset) return '—';
    const asset = this.selectedAsset as any;
    
    if (this.allocationType === 'room' && asset.locationId === this.targetId) {
      return 'Already in this room';
    }
    if (this.allocationType === 'user' && asset.assignment?.assignedToUserId === this.targetId) {
      return 'Already assigned to this user';
    }

    if (this.allocationType === 'user' && asset.assignment?.assignedToName) {
      return `Assigned to: ${asset.assignment.assignedToName}`;
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

  isAlreadyAssigned(): boolean {
    if (!this.selectedAsset) return false;
    const asset = this.selectedAsset as any;
    
    if (this.allocationType === 'room') {
      return asset.locationId === this.targetId;
    } else {
      return asset.assignment?.assignedToUserId === this.targetId;
    }
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
