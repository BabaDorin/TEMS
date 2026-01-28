import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { RoomWithHierarchy, RoomType, RoomStatus } from 'src/app/models/location/room.model';

@Component({
  selector: 'app-room-detail-modal',
  standalone: true,
  imports: [CommonModule, MatDialogModule],
  templateUrl: './room-detail-modal.component.html',
  styleUrls: ['./room-detail-modal.component.scss']
})
export class RoomDetailModalComponent {
  room: RoomWithHierarchy;

  constructor(
    public dialogRef: MatDialogRef<RoomDetailModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { room: RoomWithHierarchy },
    private router: Router
  ) {
    this.room = data.room;
  }

  close() {
    this.dialogRef.close();
  }

  viewFullPage() {
    this.dialogRef.close();
    this.router.navigate(['/locations', this.room.id]);
  }

  getTypeLabel(type: RoomType): string {
    const labels: Record<RoomType, string> = {
      [RoomType.Meeting]: 'Meeting Room',
      [RoomType.Desk]: 'Desk Area',
      [RoomType.Workshop]: 'Workshop',
      [RoomType.ServerRoom]: 'Server Room'
    };
    return labels[type] || type;
  }

  getStatusLabel(status: RoomStatus): string {
    const labels: Record<RoomStatus, string> = {
      [RoomStatus.Available]: 'Available',
      [RoomStatus.Maintenance]: 'Under Maintenance',
      [RoomStatus.Decommissioned]: 'Decommissioned'
    };
    return labels[status] || status;
  }

  getStatusClass(status: RoomStatus): string {
    if (status === RoomStatus.Available) {
      return 'bg-green-100 text-green-700';
    } else if (status === RoomStatus.Maintenance) {
      return 'bg-yellow-100 text-yellow-700';
    } else if (status === RoomStatus.Decommissioned) {
      return 'bg-red-100 text-red-700';
    }
    return 'bg-gray-100 text-gray-700';
  }

  hasAssets(): boolean {
    return !!this.room.assetCounts && Object.keys(this.room.assetCounts).length > 0;
  }

  getAssetCountEntries(): { type: string; count: number }[] {
    if (!this.room.assetCounts) return [];
    return Object.entries(this.room.assetCounts)
      .map(([type, count]) => ({ type, count }))
      .sort((a, b) => b.count - a.count);
  }

  getTotalAssetCount(): number {
    if (!this.room.assetCounts) return 0;
    return Object.values(this.room.assetCounts).reduce((sum, count) => sum + count, 0);
  }
}
