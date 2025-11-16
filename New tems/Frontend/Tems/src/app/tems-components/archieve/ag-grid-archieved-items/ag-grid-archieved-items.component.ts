import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AgGridModule } from 'ag-grid-angular';
import { propertyChanged } from 'src/app/helpers/onchanges-helper';
import { dateFormatter } from 'src/app/public/ag-grid/ag-grid-formatters';
import { ArchieveService } from 'src/app/services/archieve.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { SnackService } from '../../../services/snack.service';
import { ArchievedItem } from './../../../models/archieve/archieved-item.model';

@Component({
  selector: 'app-ag-grid-archieved-items',
  standalone: true,
  imports: [CommonModule, MatProgressBarModule, AgGridModule],
  templateUrl: './ag-grid-archieved-items.component.html',
  styleUrls: ['./ag-grid-archieved-items.component.scss']
})
export class AgGridArchievedItemsComponent extends TEMSComponent implements OnInit {

  @Input() itemsType: string;
  @Input() columnDefs: any[];
  @Input() rowData: any[];
  @Input() defaultColDef: any;

  gridApi;
  gridColumnApi;
  pagination
  paginationPageSize;
  loading: boolean = true;

  constructor(
    private archieveService: ArchieveService,
    private snackService: SnackService
  ) {
    super();
    this.pagination = true;
    this.paginationPageSize = 20;

    this.columnDefs = [
      { 
        headerName: 'Identifier', 
        field: 'identifier',
        lockPosition: true,
      },
      { 
        headerName: 'Archieved on', 
        field: 'dateArchieved', 
        valueFormatter: dateFormatter, 
      },
    ];

    this.defaultColDef = {
      flex: 1,
      minWidth: 100,
      resizable: true,
      headerCheckboxSelection: this.isFirstColumn,
      checkboxSelection: this.isFirstColumn,
    };
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(propertyChanged(changes, 'itemsType'))
      this.fetchArchievedItems();
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;

    this.fetchArchievedItems();
  }

  fetchArchievedItems(){
    this.loading = true;

    if(this.itemsType == undefined){
      this.loading = false;
      this.rowData = [];
      return;
    }

    this.subscriptions.push(
      this.archieveService.getArchievedItems(this.itemsType)
      .subscribe(result => {
        this.loading = false;
        if(this.snackService.snackIfError(result))
          return;
        
        this.rowData = result;
      })
    );
  }

  removeItem(item){
    this.gridApi.applyTransaction({ remove: [item] });
  }

  ngOnInit(): void {
  }

  isFirstColumn(params) {
    var displayedColumns = params.columnApi.getAllDisplayedColumns();
    var thisIsFirstColumn = displayedColumns[0] === params.column;
    return thisIsFirstColumn;
  }

  getSelectedNodes(){
    return this.gridApi.getSelectedNodes().map(q => q.data);
  }
}