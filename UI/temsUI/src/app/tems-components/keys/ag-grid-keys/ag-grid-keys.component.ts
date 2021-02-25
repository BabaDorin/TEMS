import { KeysService } from './../../../services/keys-service/keys.service';
import { Component, OnInit } from '@angular/core';
import { ViewKeySimplified } from 'src/app/models/key/view-key.model';

@Component({
  selector: 'app-ag-grid-keys',
  templateUrl: './ag-grid-keys.component.html',
  styleUrls: ['./ag-grid-keys.component.scss']
})
export class AgGridKeysComponent implements OnInit {

  keys: ViewKeySimplified[];

  columnDefs = [
    { field: 'identifier', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true},
    { field: 'numberOfCopies', sortable: true, filter: true },
    { field: 'description', sortable: true, filter: true },
    { field: 'isAllocated', sortable: true, filter: true },
  ];

  rowData: any;

  constructor(
    private keysService: KeysService
  ) { }

  ngOnInit(): void {
    this.rowData = this.keysService.getKeys();
  }
}
