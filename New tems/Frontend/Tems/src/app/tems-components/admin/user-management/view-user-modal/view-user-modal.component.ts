import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { UserDto } from 'src/app/models/user/user-management.model';

@Component({
  selector: 'app-view-user-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule
  ],
  templateUrl: './view-user-modal.component.html',
  styleUrls: ['./view-user-modal.component.scss']
})
export class ViewUserModalComponent implements OnInit {
  user: UserDto;

  constructor(
    public dialogRef: MatDialogRef<ViewUserModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { user: UserDto }
  ) {
    this.user = data.user;
  }

  ngOnInit(): void {}

  close() {
    this.dialogRef.close();
  }

  formatDate(dateString: string): string {
    if (!dateString) return '—';
    return new Date(dateString).toLocaleString();
  }
}
