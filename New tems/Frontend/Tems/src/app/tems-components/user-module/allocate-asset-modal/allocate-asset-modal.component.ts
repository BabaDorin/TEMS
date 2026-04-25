import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { Subject, of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, map, switchMap, takeUntil, tap } from 'rxjs/operators';
import { AssetService, AssetPageResponse } from 'src/app/services/asset.service';
import { Asset } from 'src/app/models/asset/asset.model';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

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
  imports: [CommonModule, FormsModule, MatDialogModule, CustomSelectComponent],
  templateUrl: './allocate-asset-modal.component.html',
  styleUrls: ['./allocate-asset-modal.component.scss']
})
export class AllocateAssetModalComponent implements OnInit, OnDestroy {
  allocationType: 'room' | 'user';
  targetId: string;
  targetName: string;

  assetOptions: SelectOption[] = [];
  selectedAssetId = '';
  selectedAsset: Asset | null = null;
  isSearching = false;
  isSubmitting = false;

  private readonly searchSubject = new Subject<string>();
  private readonly destroy$ = new Subject<void>();
  private assetLookup = new Map<string, Asset>();

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
      tap((searchText) => {
        this.isSearching = searchText.trim().length > 0;
      }),
      switchMap((searchText) => {
        const query = searchText.trim();
        if (!query) {
          this.assetLookup.clear();
          this.assetOptions = [];
          this.isSearching = false;
          return of([] as Asset[]);
        }

        return this.assetService.getAll(undefined, 1, 20, undefined, query).pipe(
          map((response: AssetPageResponse) => response.assets || []),
          catchError(() => of([] as Asset[]))
        );
      }),
      takeUntil(this.destroy$)
    ).subscribe((assets) => {
      this.assetLookup = new Map(assets.map((asset) => [asset.id, asset]));
      this.assetOptions = assets.map((asset) => this.toOption(asset));
      this.ensureSelectedAssetVisible();
      this.isSearching = false;
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

  onSearchChange(searchText: string) {
    this.searchSubject.next(searchText);
  }

  onAssetSelected(assetId: string) {
    this.selectedAssetId = assetId;
    this.selectedAsset = this.assetLookup.get(assetId) ?? null;
    this.ensureSelectedAssetVisible();
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

  clearSelection() {
    this.selectedAsset = null;
    this.selectedAssetId = '';
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
    }
    return asset.assignment?.assignedToUserId === this.targetId;
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

  private toOption(asset: Asset): SelectOption {
    const typeName = asset.definition?.assetTypeName || 'Unknown Type';
    const definitionName = asset.definition?.name || asset.definition?.manufacturer || '';
    const meta = definitionName ? ` · ${definitionName}` : '';

    return {
      value: asset.id,
      label: `${asset.assetTag} — ${typeName}${meta}`
    };
  }

  private ensureSelectedAssetVisible(): void {
    if (!this.selectedAsset) return;

    const exists = this.assetOptions.some((option) => option.value === this.selectedAsset!.id);
    if (!exists) {
      this.assetOptions = [this.toOption(this.selectedAsset), ...this.assetOptions];
    }
  }
}
