import { SnackService } from 'src/app/services/snack/snack.service';
import { CAN_MANAGE_ENTITIES } from './../../../models/claims';
import { TokenService } from './../../../services/token-service/token.service';
import { IViewKeySimplified } from './../../../models/key/view-key.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { KeysService } from './../../../services/keys-service/keys.service';
import { Component, Input, OnInit, OnChanges, Output, EventEmitter } from '@angular/core';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';

@Component({
  selector: 'app-ag-grid-keys',
  templateUrl: './ag-grid-keys.component.html',
  styleUrls: ['./ag-grid-keys.component.scss']
})
export class AgGridKeysComponent extends TEMSComponent implements OnInit, OnChanges {

  @Input() keys: IViewKeySimplified[] = [];
  @Input() displayAsAllocated: boolean;
  @Input() loading: boolean = false;
  
  @Output() keyReturned = new EventEmitter();
  @Output() allocateKey = new EventEmitter();

  private gridApi;
  private gridColumnApi;

  private columnDefs;
  private defaultColDef;
  private rowSelection;
  private rowData;
  private frameworkComponents: any;
  private pagination
  private paginationPageSize;

  constructor(
    private keysService: KeysService,
    private tokenService: TokenService,
    private snackService: SnackService,
  ) { 
    super();
    // enables pagination in the grid
    this.pagination = true;

    // sets 10 rows per page (default is 100)
    this.paginationPageSize = 20;

    this.frameworkComponents = {
      btnCellRendererComponent: BtnCellRendererComponent
    }
  }

  ngOnChanges(changes): void {
    this.rowData = this.keys;
  }

  ngOnInit(): void {
    if(this.displayAsAllocated){
      this.columnDefs = [
        { headerName: 'Identifier',  field: 'identifier', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true},
        { headerName: 'Room', field: 'room.label', sortable: true, filter: true },
        { headerName: 'Allocated to', field: 'allocatedTo.label', sortable: true, filter: true },
        { headerName: 'Time', field: 'timePassed', sortable: true, filter: true },
        {
          cellRenderer: 'btnCellRendererComponent',
          cellRendererParams: {
            onClick: this.return.bind(this),
            label: 'Return'
          }
        },
      ];  
    }
    else{
      this.columnDefs = [
        { headerName: 'Indentifier',  field: 'identifier', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true},
        { headerName: 'Room', field: 'room.label', sortable: true, filter: true },
        {
          cellRenderer: 'btnCellRendererComponent',
          cellRendererParams: {
            onClick: this.allocate.bind(this),
            label: 'Allocate'
          }
        },
      ];
    }

    if(this.tokenService.hasClaim(CAN_MANAGE_ENTITIES)){
      this.columnDefs.push({
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.archieve.bind(this),
          label: 'Archieve'
        }
      })
    }

    this.defaultColDef = {
      flex: 1,
      minWidth: 100,
      resizable: true,
      headerCheckboxSelection: this.isFirstColumn,
      checkboxSelection: this.isFirstColumn,
    };

    this.rowSelection = 'multiple';
  }

  return(e){
    this.subscriptions.push(
      this.keysService.markAsReturned(e.rowData.id)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          this.return;

        this.keyReturned.emit(e.rowData);
        this.gridApi.applyTransaction({ remove: [e.rowData] });
      })
    )
  }

  allocate(e){
    this.allocateKey.emit([{value: e.rowData.id, label: e.rowData.identifier}]);
  }

  archieve(e){
    if(!confirm("Are you sure you want to archive this key? It will result in archieving all of it's allocations"))
    return;

    this.subscriptions.push(
      this.keysService.archieveKey(e.rowData.id) // HERE
      .subscribe(result => {
        if(this.snackService.snack(result)){
          this.gridApi.applyTransaction({ remove: [e.rowData] });
        }
      })
    )
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;

    if(this.keys == undefined)
      this.subscriptions.push(this.keysService.getKeys()
        .subscribe(result => {
          console.log(result);
          this.rowData = result;
        }));
    else
      this.rowData = this.keys;
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
