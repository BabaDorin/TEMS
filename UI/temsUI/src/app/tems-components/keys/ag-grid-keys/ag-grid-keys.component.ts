import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChange } from '@angular/core';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';
import { SnackService } from 'src/app/services/snack.service';
import { DialogService } from '../../../services/dialog.service';
import { KeysService } from '../../../services/keys.service';
import { KeysAllocationsComponent } from '../keys-allocations/keys-allocations.component';
import { IViewKeySimplified, ViewKeySimplified } from './../../../models/key/view-key.model';
import { IOption } from './../../../models/option.model';
import { ClaimService } from './../../../services/claim.service';
import { TEMSComponent } from './../../../tems/tems.component';

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
  @Output() keyAllocated = new EventEmitter();

  private gridApi;
  private gridColumnApi;

  columnDefs;
  defaultColDef;
  rowSelection;
  rowData;
  frameworkComponents: any;
  pagination
  paginationPageSize;

  constructor(
    private keysService: KeysService,
    private snackService: SnackService,
    private dialogService: DialogService,
    private claims: ClaimService
  ) { 
    super();
    // enables pagination in the grid
    this.pagination = true;

    // sets 10 rows per page (default is 100)
    this.paginationPageSize = 20;

    this.frameworkComponents = {
      btnCellRendererComponent: BtnCellRendererComponent
    };

    this.defaultColDef = {
      resizable: true
    }
  }

  ngOnChanges(changes: { [propName: string]: SimpleChange }): void {
    if (changes['keys'] && changes['keys'].previousValue != changes['keys'].currentValue) {
      this.rowData = this.keys;
    }
  }

  ngOnInit(): void {
    this.displayAsAllocated ? this.buildColumnDefsAsAllocated() : this.buildColumnDefsAsUnallocated();

    if(this.claims.canManage || this.claims.canAllocateKeys){
      this.columnDefs.push({
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.archieve.bind(this),
          matIcon: 'delete',
          matIconClass: 'text-muted'
        }
      });
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

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;

    if(this.keys == undefined)
      this.subscriptions.push(this.keysService.getKeys()
        .subscribe(result => {
          this.rowData = result;
        }));
    else
      this.rowData = this.keys;
  }

  buildColumnDefsAsAllocated(){
    this.columnDefs = [
      { headerName: 'Identifier',  field: 'identifier', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true, resizeable: true},
      { headerName: 'Room', field: 'room.label', sortable: true, filter: true, resizeable: true },
      { headerName: 'Allocated to', field: 'allocatedTo.label', sortable: true, filter: true, resizeable: true },
      { headerName: 'Time', field: 'timePassed', sortable: true, filter: true, resizeable: true }
    ];

    if(this.claims.canAllocateKeys){
      this.columnDefs.push({
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.return.bind(this),
          label: 'Return'
        }
      });
    }
  }

  buildColumnDefsAsUnallocated(){
    this.columnDefs = [
      { headerName: 'Indentifier',  field: 'identifier', sortable: true, filter: true, checkboxSelection: true, headerCheckboxSelection: true},
      { headerName: 'Room', field: 'room.label', sortable: true, filter: true }
    ];
    
    if(this.claims.canAllocateKeys){
      this.columnDefs.push({
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.allocate.bind(this),
          label: 'Allocate'
        }
      });
    }
  }

  // Binded to the return button
  return(e){
    this.returnKeys([e.rowData]);
  }

  // Marks a collection of keys as returned, removed them from ag-grid and emits an keyReturned event for
  // each key that has been returned;
  returnKeys(keys: ViewKeySimplified[]){
    if(this.keys == undefined)
      return;
    
    keys.forEach(key => {
      this.subscriptions.push(
        this.keysService.markAsReturned(key.id)
        .subscribe(result => {
          if(this.snackService.snackIfError(result))
            return;

          this.removeKey(key);
          this.keyReturned.emit(key);
        })
      )
    })
  }

  // Binded to allocate button
  allocate(e){
    this.allocateKeys([{value: e.rowData.id, label: e.rowData.identifier}]);
  }

  // Allocates an instance of keys (via displaying the KeysAllocationComponent)
  // Emits an keyAllocated event for each allocated key.
  allocateKeys(keys: IOption[]){
    this.dialogService.openDialog(
      KeysAllocationsComponent,
      [{label: "keysAlreadySelectedOptions", value: keys }],
      () => {
        // send an event to parent component to flag one key's allocation
        // First we select the successfuly allocated keys (from the list of keys being provided as parameter)
        // and then get the actual keys.
        // After that, for each identified key, an 'keyAllocated' event is emiited, the key itself being
        // passed as parameter.
        let successfulyAllocatedKeys = keys.filter(q => q.additional != undefined);
        if(successfulyAllocatedKeys == undefined || successfulyAllocatedKeys.length == 0)
          return;

        let allocatedKeys = this.keys.filter(k => successfulyAllocatedKeys.findIndex(q => q.value == k.id) != -1);
        allocatedKeys.forEach(key => {
          key.allocatedTo = successfulyAllocatedKeys.find(q => q.value == key.id).additional;
          this.removeKey(key);
          this.keyAllocated.emit(key);
        });
      }
    )
  }

  removeKey(key: ViewKeySimplified){
    this.gridApi.applyTransaction({ remove: [key] });

    let index = this.keys.indexOf(key);
    this.keys.splice(index, 1);
    this.rowData = this.keys;
  }

  pushKey(key: ViewKeySimplified){
    this.keys.push(key);
    this.rowData = this.keys;
    this.gridApi.applyTransaction({ add: [key] });
  }

  // Binded to the archieve button
  archieve(e){
    if(!confirm("Are you sure you want to archive this key? It will result in archieving all of it's allocations"))
    return;

    this.subscriptions.push(
      this.keysService.archieveKey(e.rowData.id)
      .subscribe(result => {
        if(this.snackService.snack(result)){
          this.gridApi.applyTransaction({ remove: [e.rowData] });
        }
      })
    );
  }

  // Ag-grid helper method
  isFirstColumn(params) {
    var displayedColumns = params.columnApi.getAllDisplayedColumns();
    var thisIsFirstColumn = displayedColumns[0] === params.column;
    return thisIsFirstColumn;
  }

  // Ag-grid helper method
  getSelectedNodes(){
    return this.gridApi.getSelectedNodes().map(q => q.data);
  }
}
