import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { trigger, transition, style, animate } from '@angular/animations';
import { MatDialog } from '@angular/material/dialog';
import { AssetService } from 'src/app/services/asset.service';
import { LocationService } from 'src/app/services/location.service';
import { UserService } from 'src/app/services/user.service';
import { Asset } from 'src/app/models/asset/asset.model';
import { AssetLabelComponent } from '../../asset/asset-label/asset-label.component';
import { RoomDetailModalComponent } from '../../location-module/room-detail-modal/room-detail-modal.component';
import { ViewUserModalComponent } from '../../admin/user-management/view-user-modal/view-user-modal.component';
import { AssetTimelineComponent } from '../asset-timeline/asset-timeline.component';

@Component({
  selector: 'app-asset-detail',
  standalone: true,
  imports: [CommonModule, AssetLabelComponent, AssetTimelineComponent],
  templateUrl: './asset-detail.component.html',
  styleUrls: ['./asset-detail.component.scss'],
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
export class AssetDetailComponent implements OnInit {
  @ViewChild('assetLabel') assetLabel: AssetLabelComponent;
  
  asset: Asset | null = null;
  loading = true;
  error: string | null = null;
  activeTab: 'overview' | 'acc' | 'purchase' | 'maintenance' | 'history' = 'overview';
  showActionsDropdown = false;
  isDefinitionExpanded = true;
  private cachedAssigneeEmail: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private assetService: AssetService,
    private dialog: MatDialog,
    private locationService: LocationService,
    private userService: UserService
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadAsset(id);
    } else {
      this.error = 'No asset ID provided';
      this.loading = false;
    }
  }

  loadAsset(id: string) {
    this.loading = true;
    this.error = null;
    
    this.assetService.getById(id).subscribe({
      next: (asset) => {
        this.asset = asset;
        this.loading = false;
        this.resolveAssigneeDisplayName();
      },
      error: (error) => {
        console.error('Error loading asset:', error);
        this.error = 'Failed to load asset details';
        this.loading = false;
      }
    });
  }

  goBack() {
    this.router.navigate(['/assets/view']);
  }

  editAsset() {
    // Future: Navigate to edit view
    console.log('Edit asset:', this.asset?.id);
  }

  deleteAsset() {
    if (!this.asset) return;
    
    if (confirm(`Are you sure you want to delete asset ${this.asset.assetTag}?`)) {
      this.assetService.delete(this.asset.id).subscribe({
        next: () => {
          this.router.navigate(['/assets/view']);
        },
        error: (error) => {
          console.error('Error deleting asset:', error);
          alert('Failed to delete asset');
        }
      });
    }
  }

  getSpecificationsArray(): { key: string; value: string }[] {
    if (!this.asset?.definition?.specifications) return [];
    return this.asset.definition.specifications.map(spec => ({
      key: spec.name || spec.propertyId,
      value: this.formatSpecValue(spec.value, spec.unit)
    }));
  }

  formatSpecValue(value: any, unit?: string): string {
    if (value === null || value === undefined) return '—';
    const stringValue = typeof value === 'boolean' ? (value ? 'Yes' : 'No') : String(value);
    return unit ? `${stringValue} ${unit}` : stringValue;
  }

  getStatusClass(status: string): string {
    const normalizedStatus = status?.toUpperCase();
    switch (normalizedStatus) {
      case 'AVAILABLE':
      case 'NEW':
        return 'bg-green-100 text-green-800';
      case 'IN_USE':
      case 'ACTIVE':
        return 'bg-blue-100 text-blue-800';
      case 'UNDER_MAINTENANCE':
      case 'MAINTENANCE':
        return 'bg-yellow-100 text-yellow-800';
      case 'RETIRED':
      case 'DEFECT':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }

  getStatusLabel(status: string): string {
    const normalizedStatus = status?.toUpperCase();
    switch (normalizedStatus) {
      case 'AVAILABLE':
        return 'Available';
      case 'NEW':
        return 'New';
      case 'IN_USE':
        return 'In Use';
      case 'ACTIVE':
        return 'Active';
      case 'UNDER_MAINTENANCE':
      case 'MAINTENANCE':
        return 'Under Maintenance';
      case 'RETIRED':
        return 'Retired';
      case 'DEFECT':
        return 'Defect';
      default:
        return status ? status.charAt(0).toUpperCase() + status.slice(1).toLowerCase() : '';
    }
  }

  downloadAssetLabel(event: Event) {
    event.stopPropagation();
    if (this.assetLabel) {
      this.assetLabel.downloadLabel();
    }
  }

  getLocationString(location: any): string {
    if (!this.asset) return '—';
    
    const asset = this.asset as any;
    if (asset.locationDetails?.fullPath) {
      return asset.locationDetails.fullPath;
    }
    if (asset.locationDetails?.name) {
      return asset.locationDetails.name;
    }
    
    if (!location) return '—';
    const parts = [];
    if (location.building) parts.push(location.building);
    if (location.floor) parts.push(location.floor);
    if (location.room) parts.push(location.room);
    return parts.join(', ') || '—';
  }

  openLocationModal() {
    if (!this.asset?.locationId) return;
    this.locationService.getRoomById(this.asset.locationId).subscribe({
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

  openAssigneeModal() {
    if (!this.asset?.assignment?.assignedToUserId) return;
    this.userService.getUserById(this.asset.assignment.assignedToUserId).subscribe({
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

  hasLocationId(): boolean {
    return !!this.asset?.locationId;
  }

  hasAssignee(): boolean {
    return !!this.asset?.assignment?.assignedToUserId;
  }

  getAssigneeName(): string {
    if (!this.asset?.assignment) return '';
    const a = this.asset.assignment;
    if (a.assignedToUserName) return a.assignedToUserName;
    if (this.cachedAssigneeEmail) return this.cachedAssigneeEmail;
    return '';
  }

  private resolveAssigneeDisplayName() {
    if (!this.asset?.assignment?.assignedToUserId) return;
    if (this.asset.assignment.assignedToUserName) return;

    this.userService.getUserById(this.asset.assignment.assignedToUserId).subscribe({
      next: (user) => {
        this.cachedAssigneeEmail = user.email;
      }
    });
  }

  getAccData(): { key: string; value: string }[] {
    return [
      { key: 'CPU Temperature', value: '52°C' },
      { key: 'Memory Usage', value: '4.2 GB / 8 GB' },
      { key: 'Disk Space', value: '125 GB / 256 GB' },
      { key: 'Network Status', value: 'Connected' },
      { key: 'Last Sync', value: '2 minutes ago' },
      { key: 'Battery Health', value: '92%' },
      { key: 'Screen Brightness', value: '75%' },
      { key: 'OS Version', value: 'macOS 14.2' }
    ];
  }
}
