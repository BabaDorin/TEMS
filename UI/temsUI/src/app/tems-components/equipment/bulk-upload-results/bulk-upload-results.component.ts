import { Component, Input, OnInit } from '@angular/core';
import { SICFileUploadResult } from './../../../models/equipment/bulk-upload-result.model';

@Component({
  selector: 'app-bulk-upload-results',
  templateUrl: './bulk-upload-results.component.html',
  styleUrls: ['./bulk-upload-results.component.scss']
})
export class BulkUploadResultsComponent implements OnInit {

  @Input() bulkUploadResults: SICFileUploadResult[];

  constructor() { }

  ngOnInit(): void {
  }

}
