import { propertyChanged } from 'src/app/helpers/onchanges-helper';
import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';
import { DialogService } from '../../../services/dialog.service';
import { SnackService } from '../../../services/snack.service';
import { BooleanCellRendererComponent } from './../../../public/ag-grid/boolean-cell-renderer/boolean-cell-renderer.component';
import { EquipmentService } from './../../../services/equipment.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { EquipmentDetailsGeneralComponent } from './../equipment-details/equipment-details-general/equipment-details-general.component';
import { EquipmentFilter } from 'src/app/helpers/filters/equipment.filter';

@Component({
  selector: 'app-ag-grid-equipment',
  templateUrl: './ag-grid-equipment.component.html',
  styleUrls: ['./ag-grid-equipment.component.scss']
})

export class AgGridEquipmentComponent extends TEMSComponent implements OnChanges {

  @Input() equipmentFilter: EquipmentFilter;

  gridApi;
  gridColumnApi;
  columnDefs;
  defaultColDef;
  rowSelection;
  rowData: [];
  frameworkComponents: any;
  pagination
  paginationPageSize;
  loading: boolean = true;

  constructor(
    private equipmentService: EquipmentService,
    private dialogService: DialogService,
    private translate: TranslateService,
    private snackService: SnackService) {
    super();

    this.pagination = true;
    this.paginationPageSize = 20;

    this.frameworkComponents = {
      btnCellRendererComponent: BtnCellRendererComponent,
      booleanCellRendererComponent: BooleanCellRendererComponent
    };

    this.defaultColDef = {
      flex: 1,
      resizable: true,
      filter: true,
      sortable: true,
      width: 150,
      minWidth: 50,
    }

    this.columnDefs = [
      { 
        headerName: this.translate.instant('equipment.TEMSID'), 
        field: 'temsId',
        headerCheckboxSelection: true,
        headerCheckboxSelectionFilteredOnly: true,
        checkboxSelection: true,
        width: 180,
        lockPosition: true,
      },
      { 
        headerName: this.translate.instant('equipment.serialNumber'), 
        field: 'serialNumber',
        width: 130
      },
      { 
        headerName: this.translate.instant('equipment.definition'), 
        field: 'definition',
      },
      { 
        headerName: this.translate.instant('equipment.assignee'), 
        field: 'assignee' 
      },
      { 
        headerName: this.translate.instant('equipment.type'), 
        field: 'type' 
      },
      {
        headerName: this.translate.instant('equipment.isUsed'), 
        field: 'isUsed', 
        sortable: false,
        cellRenderer: 'booleanCellRendererComponent',
        width: 100
      },
      {
        headerName: this.translate.instant('equipment.isDefect'), 
        field: 'isDefect', 
        sortable: false,
        cellRenderer: 'booleanCellRendererComponent',
        width: 100
      },
      {
        filter: false,
        sorable: false,
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.details.bind(this),
          matIcon: 'more_horiz'
        },
        width: 100,
        suppressSizeToFit: true,
      },
      {
        cellRenderer: 'btnCellRendererComponent',
        filter: false,
        sorable: false,
        cellRendererParams: {
          onClick: this.archieve.bind(this),
          matIcon: 'delete',
          matIconClass: 'text-muted'
        },
        width: 100,
        suppressSizeToFit: true,
      }
    ];
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(propertyChanged(changes, "equipmentFilter")){
        this.fetchEquipment();
    }
  }

  details(e) {
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
        if(this.snackService.snackIfError(result))
          return;

        if(result.status == 1)
          this.gridApi.applyTransaction({ remove: [e.rowData] });
      })
    );
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    this.fetchEquipment();
  }

  fetchEquipment(){
    this.loading = true;
    this.subscriptions.push(this.equipmentService.getEquipmentSimplified(this.equipmentFilter)
    .subscribe(result => {
      this.rowData = result;
      this.loading = false;
      this.sizeToFit();
    }));
  }

  getSelectedNodes(){
    return this.gridApi.getSelectedNodes().map(q => q.data);
  }

  sizeToFit() {
    this.gridApi.sizeColumnsToFit();
  }
}
