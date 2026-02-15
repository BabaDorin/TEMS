import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { ChangelogService } from 'src/app/services/changelog.service';
import { UserService } from 'src/app/services/user.service';
import { LocationService } from 'src/app/services/location.service';
import { ChangeLogAction, ChangeLogEntry } from 'src/app/models/changelog.model';
import { ViewUserModalComponent } from '../../admin/user-management/view-user-modal/view-user-modal.component';
import { RoomDetailModalComponent } from '../../location-module/room-detail-modal/room-detail-modal.component';

@Component({
  selector: 'app-asset-timeline',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './asset-timeline.component.html'
})
export class AssetTimelineComponent implements OnChanges {
  @Input() assetId = '';

  entries: ChangeLogEntry[] = [];
  loading = true;
  error: string | null = null;
  totalCount = 0;
  pageNumber = 1;
  pageSize = 50;
  allLoaded = false;

  constructor(
    private changelogService: ChangelogService,
    private dialog: MatDialog,
    private userService: UserService,
    private locationService: LocationService
  ) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['assetId'] && this.assetId) {
      this.loadTimeline();
    }
  }

  loadTimeline() {
    this.loading = true;
    this.error = null;
    this.pageNumber = 1;
    this.allLoaded = false;

    this.changelogService.getTimeline('Asset', this.assetId, this.pageNumber, this.pageSize).subscribe({
      next: (response) => {
        this.entries = response.entries;
        this.totalCount = response.totalCount;
        this.allLoaded = this.entries.length >= this.totalCount;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load change history';
        this.loading = false;
      }
    });
  }

  loadMore() {
    this.pageNumber++;
    this.changelogService.getTimeline('Asset', this.assetId, this.pageNumber, this.pageSize).subscribe({
      next: (response) => {
        this.entries = [...this.entries, ...response.entries];
        this.totalCount = response.totalCount;
        this.allLoaded = this.entries.length >= this.totalCount;
      }
    });
  }

  isLatest(index: number): boolean {
    return index === 0;
  }

  isFirstEver(index: number): boolean {
    return this.allLoaded && index === this.entries.length - 1;
  }

  getUserName(entry: ChangeLogEntry): string | null {
    return entry.details?.['userName'] ?? null;
  }

  getLocationName(entry: ChangeLogEntry): string | null {
    return entry.details?.['locationName'] ?? null;
  }

  getActionIcon(action: ChangeLogAction): string {
    switch (action) {
      case 'AssetCreated':
        return 'M12 4v16m8-8H4';
      case 'AssetDeleted':
        return 'M6 18L18 6M6 6l12 12';
      case 'AssetUpdated':
        return 'M16.862 4.487l1.687-1.688a1.875 1.875 0 112.652 2.652L10.582 16.07a4.5 4.5 0 01-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 011.13-1.897l8.932-8.931z';
      case 'AssetAssignedToUser':
      case 'UserAssetAssigned':
        return 'M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0';
      case 'AssetUnassignedFromUser':
      case 'UserAssetUnassigned':
        return 'M22 10.5h-6m-2.25-4.125a3.375 3.375 0 11-6.75 0 3.375 3.375 0 016.75 0zM4 19.235v-.11a6.375 6.375 0 0112.75 0v.109';
      case 'AssetAssignedToLocation':
      case 'LocationAssetAssigned':
        return 'M15 10.5a3 3 0 11-6 0 3 3 0 016 0z M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1115 0z';
      case 'AssetUnassignedFromLocation':
      case 'LocationAssetUnassigned':
        return 'M15 10.5a3 3 0 11-6 0 3 3 0 016 0z M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1115 0z';
      default:
        return 'M11.25 11.25l.041-.02a.75.75 0 011.063.852l-.708 2.836a.75.75 0 001.063.853l.041-.021M21 12a9 9 0 11-18 0 9 9 0 0118 0z';
    }
  }

  getActionColor(action: ChangeLogAction): string {
    switch (action) {
      case 'AssetCreated':
        return 'bg-green-100 text-green-600 dark:bg-green-500/20 dark:text-green-400';
      case 'AssetDeleted':
        return 'bg-red-100 text-red-600 dark:bg-red-500/20 dark:text-red-400';
      case 'AssetUpdated':
        return 'bg-blue-100 text-blue-600 dark:bg-blue-500/20 dark:text-blue-400';
      case 'AssetAssignedToUser':
      case 'UserAssetAssigned':
        return 'bg-purple-100 text-purple-600 dark:bg-purple-500/20 dark:text-purple-400';
      case 'AssetUnassignedFromUser':
      case 'UserAssetUnassigned':
        return 'bg-orange-100 text-orange-600 dark:bg-orange-500/20 dark:text-orange-400';
      case 'AssetAssignedToLocation':
      case 'LocationAssetAssigned':
        return 'bg-teal-100 text-teal-600 dark:bg-teal-500/20 dark:text-teal-400';
      case 'AssetUnassignedFromLocation':
      case 'LocationAssetUnassigned':
        return 'bg-amber-100 text-amber-600 dark:bg-amber-500/20 dark:text-amber-400';
      default:
        return 'bg-gray-100 text-gray-600 dark:bg-gray-500/20 dark:text-gray-400';
    }
  }

  getFieldChanges(entry: ChangeLogEntry): { fieldName: string; oldValue: string; newValue: string }[] {
    if (entry.action !== 'AssetUpdated' || !entry.details?.['changes']) return [];
    return entry.details['changes'] as { fieldName: string; oldValue: string; newValue: string }[];
  }

  getReason(entry: ChangeLogEntry): string | null {
    return entry.details?.['reason'] ?? null;
  }

  openUserModal(userId: string | null | undefined) {
    if (!userId) return;
    this.userService.getUserById(userId).subscribe({
      next: (user) => {
        this.dialog.open(ViewUserModalComponent, {
          width: '520px',
          maxWidth: '95vw',
          data: { user },
          panelClass: 'custom-dialog-container'
        });
      }
    });
  }

  openLocationModal(locationId: string | null | undefined) {
    if (!locationId) return;
    this.locationService.getRoomById(locationId).subscribe({
      next: (room) => {
        this.dialog.open(RoomDetailModalComponent, {
          width: '520px',
          maxWidth: '95vw',
          data: { room },
          panelClass: 'custom-dialog-container'
        });
      }
    });
  }

  formatTimestamp(timestamp: string): string {
    const date = new Date(timestamp);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    if (diffHours < 24) return `${diffHours}h ago`;
    if (diffDays < 7) return `${diffDays}d ago`;

    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: date.getFullYear() !== now.getFullYear() ? 'numeric' : undefined
    });
  }

  formatFullTimestamp(timestamp: string): string {
    return new Date(timestamp).toLocaleString('en-US', {
      month: 'long', day: 'numeric', year: 'numeric',
      hour: 'numeric', minute: '2-digit', hour12: true
    });
  }

  get hasMore(): boolean {
    return this.entries.length < this.totalCount;
  }
}
