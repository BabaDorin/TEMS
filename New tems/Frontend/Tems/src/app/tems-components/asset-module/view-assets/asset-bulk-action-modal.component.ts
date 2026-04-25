import { CommonModule } from '@angular/common';
import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { Subject, Subscription, firstValueFrom, of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { Asset } from 'src/app/models/asset/asset.model';
import { UserLookupDto } from 'src/app/models/user/user-management.model';
import { AssetService } from 'src/app/services/asset.service';
import { LocationService } from 'src/app/services/location.service';
import { UserService } from 'src/app/services/user.service';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

type BulkActionMode = 'user' | 'room';

interface BulkActionModalData {
  mode: BulkActionMode;
  assets: Asset[];
}

interface OptionItem extends SelectOption {
  name?: string;
}

@Component({
  selector: 'app-asset-bulk-action-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule, CustomSelectComponent],
  templateUrl: './asset-bulk-action-modal.component.html'
})
export class AssetBulkActionModalComponent implements OnInit, OnDestroy {
  mode: BulkActionMode;
  assets: Asset[];
  options: OptionItem[] = [];
  selectedTargetId = '';
  selectedTargetOption: OptionItem | null = null;
  loading = true;
  submitting = false;
  errorMessage = '';
  private readonly dropdownTake = 12;
  private readonly searchTerms$ = new Subject<string>();
  private readonly subscriptions = new Subscription();

  constructor(
    public dialogRef: MatDialogRef<AssetBulkActionModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: BulkActionModalData,
    private assetService: AssetService,
    private userService: UserService,
    private locationService: LocationService
  ) {
    this.mode = data.mode;
    this.assets = data.assets ?? [];
  }

  get title(): string {
    return this.mode === 'user' ? 'Assign Assets to User' : 'Move Assets to Room';
  }

  get submitLabel(): string {
    if (this.submitting) {
      return this.mode === 'user' ? 'Assigning...' : 'Moving...';
    }
    return this.mode === 'user' ? 'Assign' : 'Move';
  }

  ngOnInit(): void {
    if (this.mode === 'user') {
      this.initUserLookup();
      this.searchTerms$.next('');
      return;
    }

    this.loadRoomOptions();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
    this.searchTerms$.complete();
  }

  close(): void {
    this.dialogRef.close();
  }

  onTargetSearchChange(searchText: string): void {
    if (this.mode === 'user') {
      this.searchTerms$.next(searchText);
    }
  }

  onTargetChange(targetId: string): void {
    this.selectedTargetId = targetId;
    this.selectedTargetOption = this.options.find((option) => option.value === targetId) ?? null;
  }

  async submit(): Promise<void> {
    if (!this.selectedTargetId || this.assets.length === 0 || this.submitting) return;

    this.submitting = true;
    this.errorMessage = '';

    try {
      const targetName = this.selectedTargetOption?.name
        ?? this.selectedTargetOption?.label
        ?? this.options.find((o) => o.value === this.selectedTargetId)?.label
        ?? '';

      await Promise.all(
        this.assets.map((asset) => {
          if (this.mode === 'user') {
            return firstValueFrom(this.assetService.assignToUser(asset.id, this.selectedTargetId, targetName));
          }
          return firstValueFrom(this.assetService.assignToRoom(asset.id, this.selectedTargetId));
        })
      );

      this.dialogRef.close({ success: true });
    } catch {
      this.errorMessage = `Failed to ${this.mode === 'user' ? 'assign' : 'move'} one or more assets.`;
      this.submitting = false;
    }
  }

  private initUserLookup(): void {
    this.subscriptions.add(
      this.searchTerms$.pipe(
        debounceTime(250),
        distinctUntilChanged(),
        tap(() => {
          this.loading = true;
          this.errorMessage = '';
        }),
        switchMap((searchText) =>
          this.userService.searchUsersByName(searchText, this.dropdownTake).pipe(
            catchError(() => {
              this.errorMessage = 'Failed to load users.';
              return of([] as UserLookupDto[]);
            })
          )
        )
      ).subscribe((users) => {
      this.options = this.mapUserOptions(users);
      this.syncSelectedOption();
      this.loading = false;
      })
    );
  }

  private loadRoomOptions(): void {
    this.loading = true;
    this.errorMessage = '';

    this.locationService.getRoomsWithHierarchy(undefined, undefined, 1, 500).subscribe({
      next: (response) => {
        this.options = (response.data ?? [])
          .map((room) => ({ value: room.id, label: this.getRoomLabel(room) }))
          .sort((left, right) => left.label.localeCompare(right.label));
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load rooms.';
        this.loading = false;
      }
    });
  }

  private getRoomLabel(room: { id: string; siteName?: string; buildingName?: string; name?: string }): string {
    const parts = [room.siteName, room.buildingName, room.name].filter(Boolean);
    return parts.join(' > ') || room.id;
  }

  private mapUserOptions(users: UserLookupDto[]): OptionItem[] {
    return users.map((user) => ({
      value: user.id,
      label: user.displayName || user.name || user.email || user.id,
      name: user.name || user.displayName || user.email || user.id
    }));
  }

  private syncSelectedOption(): void {
    if (!this.selectedTargetId) {
      this.selectedTargetOption = null;
      return;
    }

    const selected = this.options.find((option) => option.value === this.selectedTargetId);
    if (selected) {
      this.selectedTargetOption = selected;
      return;
    }

    if (this.selectedTargetOption) {
      this.options = [this.selectedTargetOption, ...this.options.filter((option) => option.value !== this.selectedTargetOption?.value)];
    }
  }
}
