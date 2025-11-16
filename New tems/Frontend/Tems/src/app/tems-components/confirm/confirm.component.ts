import { Component, Inject, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-confirm',
  standalone: true,
  imports: [CommonModule, MatButtonModule, TranslateModule],
  templateUrl: './confirm.component.html',
  styleUrls: ['./confirm.component.scss']
})
export class ConfirmComponent {
  message: string;
  confirmButtonText: string = 'OK';
  cancelButtonText: string = 'Cancel';

  constructor(
    @Optional() public dialogRef: MatDialogRef<ConfirmComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.message = data?.message || '';
    this.confirmButtonText = data?.confirmButtonText || 'OK';
    this.cancelButtonText = data?.cancelButtonText || 'Cancel';
  }

  ok() {
    this.dialogRef?.close(true);
  }

  cancel() {
    this.dialogRef?.close(false);
  }
}
