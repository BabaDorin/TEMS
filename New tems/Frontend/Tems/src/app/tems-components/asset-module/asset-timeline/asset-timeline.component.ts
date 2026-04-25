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
  loadedRawCount = 0;
  private locationNameCache = new Map<string, string>();
  private resolvingLocationIds = new Set<string>();

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
        this.loadedRawCount = response.entries.length;
        this.entries = this.mergeAssignmentTransitions(response.entries);
        this.resolveMissingLocationNames(this.entries);
        this.totalCount = response.totalCount;
        this.allLoaded = this.loadedRawCount >= this.totalCount;
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
        this.loadedRawCount += response.entries.length;
        this.entries = this.mergeAssignmentTransitions([...this.entries, ...response.entries]);
        this.resolveMissingLocationNames(this.entries);
        this.totalCount = response.totalCount;
        this.allLoaded = this.loadedRawCount >= this.totalCount;
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
    const locationId = this.getLocationId(entry);
    if (locationId) {
      const resolved = this.locationNameCache.get(locationId);
      if (resolved) return resolved;
    }

    const locationName = entry.details?.['locationName'] ?? null;
    if (typeof locationName !== 'string') return null;
    return locationName;
  }

  getDisplayDescription(entry: ChangeLogEntry): string {
    if (entry.action === 'AssetAssignedToUser') {
      const nextUserName = entry.details?.['userName'];
      const previousUserName = entry.details?.['previousUserName'];
      if (nextUserName && previousUserName) {
        return `Asset moved from ${previousUserName} to ${nextUserName}`;
      }
    }

    if (entry.action === 'AssetAssignedToLocation') {
      const nextLocationName = entry.details?.['locationName'];
      const previousLocationName = entry.details?.['previousLocationName'];
      if (nextLocationName && previousLocationName) {
        return `Asset moved from location '${previousLocationName}' to '${nextLocationName}'`;
      }
    }

    const locationId = this.getLocationId(entry);
    const resolvedLocationName = locationId ? this.locationNameCache.get(locationId) : null;

    if (!resolvedLocationName) return entry.description;

    let result = entry.description;
    const rawLocationName = entry.details?.['locationName'];

    if (typeof rawLocationName === 'string' && rawLocationName.trim().length > 0) {
      result = result.replace(rawLocationName, resolvedLocationName);
    }

    if (locationId && result.includes(locationId)) {
      result = result.replace(locationId, resolvedLocationName);
    }

    return result;
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

  private getLocationId(entry: ChangeLogEntry): string | null {
    const locationId = entry.references?.['locationId'];
    if (!locationId || typeof locationId !== 'string') return null;
    return locationId;
  }

  private shouldResolveLocationName(entry: ChangeLogEntry): boolean {
    const locationId = this.getLocationId(entry);
    if (!locationId) return false;
    if (this.locationNameCache.has(locationId)) return false;

    const locationName = entry.details?.['locationName'];
    if (typeof locationName !== 'string' || locationName.trim().length === 0) {
      return true;
    }

    return locationName === locationId || this.looksLikeId(locationName);
  }

  private looksLikeId(value: string): boolean {
    const trimmed = value.trim();
    if (trimmed.includes(' ')) return false;
    return /^[a-f0-9-]{20,}$/i.test(trimmed);
  }

  private resolveMissingLocationNames(entries: ChangeLogEntry[]): void {
    entries
      .filter((entry) => this.shouldResolveLocationName(entry))
      .forEach((entry) => {
        const locationId = this.getLocationId(entry);
        if (!locationId || this.resolvingLocationIds.has(locationId)) return;

        this.resolvingLocationIds.add(locationId);
        this.locationService.getRoomById(locationId).subscribe({
          next: (room) => {
            const locationName = [room.buildingName, room.name].filter(Boolean).join(' / ') || room.name || locationId;
            this.locationNameCache.set(locationId, locationName);
            this.resolvingLocationIds.delete(locationId);
          },
          error: () => {
            this.resolvingLocationIds.delete(locationId);
          }
        });
      });
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
    return this.loadedRawCount < this.totalCount;
  }

  private mergeAssignmentTransitions(entries: ChangeLogEntry[]): ChangeLogEntry[] {
    const merged: ChangeLogEntry[] = [];
    let i = 0;

    while (i < entries.length) {
      const current = entries[i];
      const next = entries[i + 1];

      const mergedUser = this.tryMergeUserReassignment(current, next)
        ?? this.tryMergeUserReassignment(next, current);
      if (mergedUser) {
        merged.push(mergedUser);
        i += 2;
        continue;
      }

      const mergedLocation = this.tryMergeLocationMove(current, next)
        ?? this.tryMergeLocationMove(next, current);
      if (mergedLocation) {
        merged.push(mergedLocation);
        i += 2;
        continue;
      }

      merged.push(current);
      i += 1;
    }

    return merged;
  }

  private tryMergeUserReassignment(
    assigned?: ChangeLogEntry,
    unassigned?: ChangeLogEntry
  ): ChangeLogEntry | null {
    if (!assigned || !unassigned) return null;
    if (assigned.action !== 'AssetAssignedToUser' || unassigned.action !== 'AssetUnassignedFromUser') return null;
    if (!this.isWithinMergeWindow(assigned.timestamp, unassigned.timestamp)) return null;

    const assignedAssetId = assigned.references?.['assetId'];
    const unassignedAssetId = unassigned.references?.['assetId'];
    if (!assignedAssetId || assignedAssetId !== unassignedAssetId) return null;

    const previousUserId = assigned.references?.['previousUserId'];
    const unassignedUserId = unassigned.references?.['userId'];
    if (previousUserId && unassignedUserId && previousUserId !== unassignedUserId) return null;

    const movedFromName = unassigned.details?.['userName'] || assigned.details?.['previousUserName'] || 'previous user';
    const movedToName = assigned.details?.['userName'] || 'new user';

    return {
      ...assigned,
      id: `${assigned.id}__merged__${unassigned.id}`,
      description: `Asset moved from ${movedFromName} to ${movedToName}`,
      references: {
        ...assigned.references,
        previousUserId: previousUserId || unassignedUserId || null
      },
      details: {
        ...(assigned.details || {}),
        previousUserName: movedFromName,
        userName: movedToName
      }
    };
  }

  private tryMergeLocationMove(
    assigned?: ChangeLogEntry,
    unassigned?: ChangeLogEntry
  ): ChangeLogEntry | null {
    if (!assigned || !unassigned) return null;
    if (assigned.action !== 'AssetAssignedToLocation' || unassigned.action !== 'AssetUnassignedFromLocation') return null;
    if (!this.isWithinMergeWindow(assigned.timestamp, unassigned.timestamp)) return null;

    const assignedAssetId = assigned.references?.['assetId'];
    const unassignedAssetId = unassigned.references?.['assetId'];
    if (!assignedAssetId || assignedAssetId !== unassignedAssetId) return null;

    const previousLocationId = assigned.references?.['previousLocationId'];
    const unassignedLocationId = unassigned.references?.['locationId'];
    if (previousLocationId && unassignedLocationId && previousLocationId !== unassignedLocationId) return null;

    const movedFromName = unassigned.details?.['locationName'] || assigned.details?.['previousLocationName'] || 'previous location';
    const movedToName = assigned.details?.['locationName'] || 'new location';

    return {
      ...assigned,
      id: `${assigned.id}__merged__${unassigned.id}`,
      description: `Asset moved from location '${movedFromName}' to '${movedToName}'`,
      references: {
        ...assigned.references,
        previousLocationId: previousLocationId || unassignedLocationId || null
      },
      details: {
        ...(assigned.details || {}),
        previousLocationName: movedFromName,
        locationName: movedToName
      }
    };
  }

  private isWithinMergeWindow(firstTimestamp: string, secondTimestamp: string): boolean {
    const first = new Date(firstTimestamp).getTime();
    const second = new Date(secondTimestamp).getTime();
    if (Number.isNaN(first) || Number.isNaN(second)) return false;
    return Math.abs(first - second) <= 120000;
  }
}
