import { BooleanCellRendererComponent } from './../../../public/ag-grid/boolean-cell-renderer/boolean-cell-renderer.component';
import { EquipmentDetailsGeneralComponent } from './../equipment-details/equipment-details-general/equipment-details-general.component';
import { DialogService } from '../../../services/dialog.service';
import { SnackService } from '../../../services/snack.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { EquipmentService } from './../../../services/equipment.service';
import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';

@Component({
  selector: 'app-ag-grid-equipment',
  templateUrl: './ag-grid-equipment.component.html',
  styleUrls: ['./ag-grid-equipment.component.scss']
})

export class AgGridEquipmentComponent extends TEMSComponent implements OnInit, OnChanges {

  @Input() includeDerived = false;
  @Input() rooms: string[];
  @Input() personnel: string[];

  private gridApi;
  private gridColumnApi;

  private columnDefs;
  private defaultColDef;
  private rowSelection;
  private rowData: [];
  private frameworkComponents: any;
  private pagination
  private paginationPageSize;
  loading: boolean = true;

  constructor(
    private equipmentService: EquipmentService,
    private dialogService: DialogService,
    private snackService: SnackService) {
    super();

    // enables pagination in the grid
    this.pagination = true;

    this.paginationPageSize = 20;

    this.frameworkComponents = {
      btnCellRendererComponent: BtnCellRendererComponent,
      booleanCellRendererComponent: BooleanCellRendererComponent
    }

    this.columnDefs = [
      { field: 'temsId', sortable: true, filter: true },
      { field: 'serialNumber', sortable: true, filter: true },
      { field: 'definition', sortable: true, filter: true },
      { field: 'assignee', sortable: true, filter: true },
      { field: 'type', sortable: true, filter: true },
      {
        field: 'isUsed', sortable: false, filter: true,
        cellRenderer: 'booleanCellRendererComponent',
      },
      {
        field: 'isDefect', sortable: false, filter: true,
        cellRenderer: 'booleanCellRendererComponent',
      },
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

  ngOnChanges(changes: SimpleChanges): void {
    if(this.cancelFirstOnChange){
      this.cancelFirstOnChange = false;
      return;
    }
    
    console.log('changed');
    this.fetchEquipments();
  }

  details(e) {
    console.log('data from ag-grid-equipment:');
    console.log(e);

    this.dialogService.openDialog(
      EquipmentDetailsGeneralComponent,
      [
        { label: "displayViewMore", value: true },
        { label: "equipmentId", value: e.rowData.id },
      ]
    )
  }

  archieve(e){
    if(!confirm("Are you sure you want to archive this item? It will result in archieving all of it's logs and allocations"))
    return;

    this.subscriptions.push(
      this.equipmentService.archieveEquipment(e.rowData.id)
      .subscribe(result => {
        if(this.snackService.snack(result)){
          this.gridApi.applyTransaction({ remove: [e.rowData] });
        }
      })
    )
  }

  ngOnInit() {

  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;

    this.fetchEquipments();
  }

  fetchEquipments(){
    this.loading = true;
    let entityCollection = {
      roomIds: this.rooms,
      personnelIds: this.personnel
    }

    this.subscriptions.push(this.equipmentService.getEquipmentSimplified(20, 20, !this.includeDerived, entityCollection)
    .subscribe(result => {
      console.log(result);
      this.rowData = result;
      this.loading = false;
    }));
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
