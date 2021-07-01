import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { dateFormatter } from 'src/app/public/ag-grid/ag-grid-formatters';
import { ArchieveService } from 'src/app/services/archieve.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { SnackService } from '../../../services/snack.service';
import { ArchievedItem } from './../../../models/archieve/archieved-item.model';

@Component({
  selector: 'app-ag-grid-archieved-items',
  templateUrl: './ag-grid-archieved-items.component.html',
  styleUrls: ['./ag-grid-archieved-items.component.scss']
})
export class AgGridArchievedItemsComponent extends TEMSComponent implements OnInit, OnChanges {

  @Input() items: ArchievedItem[];
  @Input() itemsType: string;

  public gridApi;
  private gridColumnApi;

  public columnDefs;
  public defaultColDef;
  public rowSelection;
  public rowData: ArchievedItem[];
  public pagination
  public paginationPageSize;
  loading: boolean = true;

  constructor(
    private archieveService: ArchieveService,
    private snackService: SnackService
  ) {
    super();
    // enables pagination in the grid
    this.pagination = true;

    // sets 10 rows per page (default is 100)
    this.paginationPageSize = 20;

    this.columnDefs = [
      { headerName: 'Identifier', field: 'identifier', sortable: true, filter: true },
      { headerName: 'Archieved on', field: 'dateArchieved', valueFormatter: dateFormatter, sortable: true, filter: true },
    ];

    this.defaultColDef = {
      flex: 1,
      minWidth: 100,
      resizable: true,
      headerCheckboxSelection: this.isFirstColumn,
      checkboxSelection: this.isFirstColumn,
    };

    this.rowSelection = 'multiple';
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(this.cancelFirstOnChange){
      this.cancelFirstOnChange = false;
      return;
    }

    this.fetchArchievedItems();
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;

    this.fetchArchievedItems();
  }

  fetchArchievedItems(){
    if(this.itemsType == undefined){
      this.loading = false;
      this.rowData = [];
      return;
    }

    this.loading = true;
    this.subscriptions.push(
      this.archieveService.getArchievedItems(this.itemsType)
      .subscribe(result => {
        this.loading = false;
        if(this.snackService.snackIfError(result))
          return;
        
        this.rowData = result;
      })
    )
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