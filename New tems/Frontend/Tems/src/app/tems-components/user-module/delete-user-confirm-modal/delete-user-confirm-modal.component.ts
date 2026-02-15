import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { UserDto } from 'src/app/models/user/user-management.model';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-delete-user-confirm-modal',
  standalone: true,
  imports: [CommonModule, MatDialogModule],
  templateUrl: './delete-user-confirm-modal.component.html',
  styleUrls: ['./delete-user-confirm-modal.component.scss']
})
export class DeleteUserConfirmModalComponent implements OnInit {
  user: UserDto;
  assetCount = 0;
  isLoadingCount = true;
  isDeleting = false;
  error: string | null = null;

  constructor(
    public dialogRef: MatDialogRef<DeleteUserConfirmModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { user: UserDto },
    private userService: UserService
  ) {
    this.user = data.user;
  }

  ngOnInit() {
    this.loadAssetCount();
  }

  loadAssetCount() {
    this.isLoadingCount = true;
    this.userService.getUserAssetCount(this.user.id).subscribe({
      next: (response) => {
        this.assetCount = response.count;
        this.isLoadingCount = false;
      },
      error: () => {
        this.assetCount = 0;
        this.isLoadingCount = false;
      }
    });
  }

  close() {
    this.dialogRef.close();
  }

  confirmDelete() {
    this.isDeleting = true;
    this.error = null;

    this.userService.deleteManagedUser(this.user.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.dialogRef.close({ deleted: true });
        } else {
          this.error = response.message || 'Failed to delete user';
          this.isDeleting = false;
        }
      },
      error: (err) => {
        console.error('Failed to delete user:', err);
        this.error = 'Failed to delete user. Please try again.';
        this.isDeleting = false;
      }
    });
  }

  getUserFullName(): string {
    const parts = [];
    if (this.user.firstName) parts.push(this.user.firstName);
    if (this.user.lastName) parts.push(this.user.lastName);
    return parts.length > 0 ? parts.join(' ') : this.user.username;
  }
}
