import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { Subject, of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, map, switchMap, takeUntil, tap } from 'rxjs/operators';
import { AssetService, AssetPageResponse } from 'src/app/services/asset.service';
import { Asset } from 'src/app/models/asset/asset.model';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

@Component({
  selector: 'app-add-asset-to-room-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule, CustomSelectComponent],
  templateUrl: './add-asset-to-room-modal.component.html',
  styleUrls: ['./add-asset-to-room-modal.component.scss']
})
export class AddAssetToRoomModalComponent implements OnInit, OnDestroy {
  roomId: string;
  roomName: string;

  assetOptions: SelectOption[] = [];
  selectedAssetId = '';
  selectedAsset: Asset | null = null;
  isSearching = false;
  isSubmitting = false;

  private readonly searchSubject = new Subject<string>();
  private readonly destroy$ = new Subject<void>();
  private assetLookup = new Map<string, Asset>();

  constructor(
    public dialogRef: MatDialogRef<AddAssetToRoomModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { roomId: string; roomName: string },
    private assetService: AssetService
  ) {
    this.roomId = data.roomId;
    this.roomName = data.roomName;
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

  clearSelection() {
    this.selectedAsset = null;
    this.selectedAssetId = '';
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
