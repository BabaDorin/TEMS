import { Component, Inject, Input, OnInit, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { TranslateModule } from '@ngx-translate/core';
import { SICFileUploadResult } from './../../../models/asset/bulk-upload-result.model';

@Component({
  selector: 'app-bulk-upload-results',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatTabsModule, TranslateModule],
  templateUrl: './bulk-upload-results.component.html',
  styleUrls: ['./bulk-upload-results.component.scss']
})
export class BulkUploadResultsComponent implements OnInit {

  @Input() bulkUploadResults: SICFileUploadResult[];

  constructor(
    @Optional() public dialogRef: MatDialogRef<BulkUploadResultsComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit(): void {
  }

}
