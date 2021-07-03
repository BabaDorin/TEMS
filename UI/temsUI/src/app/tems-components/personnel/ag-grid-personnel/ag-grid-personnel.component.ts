import { Component, OnInit } from '@angular/core';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';
import { DialogService } from 'src/app/services/dialog.service';
import { SnackService } from 'src/app/services/snack.service';
import { PersonnelService } from '../../../services/personnel.service';
import { ViewPersonnelSimplified } from './../../../models/personnel/view-personnel-simplified.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { PersonnelDetailsGeneralComponent } from './../personnel-details-general/personnel-details-general.component';

@Component({
  selector: 'app-ag-grid-personnel',
  templateUrl: './ag-grid-personnel.component.html',
  styleUrls: ['./ag-grid-personnel.component.scss']
})
export class AgGridPersonnelComponent extends TEMSComponent implements OnInit {

  private gridApi;
  private gridColumnApi;

  columnDefs;
  defaultColDef;
  rowSelection;
  rowData: [];
  frameworkComponents: any;
  pagination
  paginationPageSize;
  loading: boolean = true;

  personnel: ViewPersonnelSimplified[];

  constructor(
    private personnelService: PersonnelService,
    private dialogService: DialogService,
    private snackService: SnackService
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

    this.columnDefs = [
      { field: 'name', sortable: true, filter: true, checkboxSelection: true, resizeable: true},
      { field: 'positions', sortable: true, filter: true, resizeable: true },
      { field: 'allocatedEquipments', sortable: true, filter: true, resizeable: true },
      { field: 'activeTickets', sortable: true, filter: true, resizeable: true },
      {
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.details.bind(this),
          matIcon: 'more_horiz'
        }
      },
      {
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.archieve.bind(this),
          matIcon: 'delete',
          matIconClass: 'text-muted'
        }
      }
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

  ngOnInit(): void {
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;

    this.fetchPersonnel();
  }

  fetchPersonnel(){
    this.loading = true;
    this.subscriptions.push(this.personnelService.getPersonnelSimplified(20, 20)
      .subscribe(result => {
        this.rowData = result;
        this.loading = false;
        this.autoSizeAll();
      }));
  }

  autoSizeAll() {
    var allColumnIds = [];
    this.gridColumnApi.getAllColumns().forEach(function (column) {
      allColumnIds.push(column.colId);
    });
    this.gridColumnApi.autoSizeColumns(allColumnIds, false);
  }

  isFirstColumn(params) {
    var displayedColumns = params.columnApi.getAllDisplayedColumns();
    var thisIsFirstColumn = displayedColumns[0] === params.column;
    return thisIsFirstColumn;
  }

  getSelectedNodes(){
    return this.gridApi.getSelectedNodes().map(q => q.data);
  }

  details(e) {
    this.dialogService.openDialog(
      PersonnelDetailsGeneralComponent,
      [
        { label: "displayViewMore", value: true },
        { label: "personnelId", value: e.rowData.id },
      ]
    )
  }

  archieve(e){
    if(!confirm("Are you sure you want to archive this item? It will result in archieving all of it's logs and allocations"))
    return;

    this.subscriptions.push(
      this.personnelService.archievePersonnel(e.rowData.id)
      .subscribe(result => {
        if(this.snackService.snack(result)){
          this.gridApi.applyTransaction({ remove: [e.rowData] });
        }
      })
    )
  }
}
