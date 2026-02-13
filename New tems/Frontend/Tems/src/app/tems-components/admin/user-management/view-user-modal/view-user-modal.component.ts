import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { UserDto } from 'src/app/models/user/user-management.model';

@Component({
  selector: 'app-view-user-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule
  ],
  templateUrl: './view-user-modal.component.html',
  styleUrls: ['./view-user-modal.component.scss']
})
export class ViewUserModalComponent {
  user: UserDto;

  constructor(
    public dialogRef: MatDialogRef<ViewUserModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { user: UserDto },
    private router: Router
  ) {
    this.user = data.user;
  }

  get totalAssetCount(): number {
    if (!this.user.assetCounts) return 0;
    return Object.values(this.user.assetCounts).reduce((sum, count) => sum + count, 0);
  }

  get assetCountEntries(): { type: string; count: number }[] {
    if (!this.user.assetCounts) return [];
    return Object.entries(this.user.assetCounts).map(([type, count]) => ({ type, count }));
  }

  close() {
    this.dialogRef.close();
  }

  viewFullPage() {
    this.dialogRef.close();
    this.router.navigate(['/users', this.user.id]);
  }

  formatDate(dateString: string): string {
    if (!dateString) return '—';
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}
